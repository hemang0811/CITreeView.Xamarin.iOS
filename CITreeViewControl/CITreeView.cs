using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreAnimation;

namespace CITreeViewControl
{
    public interface ITreeViewDataSource : INSObjectProtocol
    {
        UITableViewCell GetCell(CITreeView treeView, NSIndexPath indexPath, CITreeViewNode treeViewNode);
        CITreeViewData[] GetChildrenForNode(CITreeViewData node);
        CITreeViewData[] GetData();
    }

    public interface ITreeViewDelegate : INSObjectProtocol
    {
        nfloat GetHeightForRow(CITreeView treeView, NSIndexPath indexPath, CITreeViewNode treeViewNode);
        void RowSelected(CITreeView treeView, CITreeViewNode treeViewNode, NSIndexPath indexPath);
        void RowDeSelected(CITreeView treeView, CITreeViewNode treeViewNode, NSIndexPath indexPath);
        void WillExpand(CITreeViewNode treeViewNode, NSIndexPath indexPath);
        void DidExpand(CITreeViewNode treeViewNode, NSIndexPath indexPath);
        void WillCollapse(CITreeViewNode treeViewNode, NSIndexPath indexPath);
        void DidCollapse(CITreeViewNode treeViewNode, NSIndexPath indexPath);
    }
    public partial class CITreeView : UITableView, IUITableViewDataSource, IUITableViewDelegate, ITreeControllerDelegate
    {
        private CITreeController treeController = new CITreeController(new List<CITreeViewNode>());
        private CITreeViewNode selectedTreeViewNode = null;
        public ITreeViewDataSource treeViewDataSource { get; set; }
        public ITreeViewDelegate treeViewDelegate { get; set; }

        public bool CollapseNoneSelectedRows = false;

        private List<CITreeViewNode> mainData = new List<CITreeViewNode>();

        public CITreeView (IntPtr handle) : base (handle)
        {
            CommonInit();
        }
        public CITreeView(CGRect frame, UITableViewStyle style) : base(frame, style)
        {
            CommonInit();
        }

        public CITreeView(NSCoder nSCoder) : base(nSCoder)
        {
            CommonInit();
        }
        void CommonInit()
        {
            base.Delegate = this;
            base.DataSource = this;
            treeController.Delegate = this as ITreeControllerDelegate;
            this.BackgroundColor = UIColor.Clear;
        }
        public override void ReloadData()
        {
            if (this.treeViewDataSource == null)
            {
                mainData = new List<CITreeViewNode>();
                return;
            }
            mainData = new List<CITreeViewNode>();
            treeController.TreeViewNodes.Clear();
            foreach (var item in treeViewDataSource.GetData())
            {
                treeController.AddTreeViewNode(item);
            }
            mainData = treeController.TreeViewNodes;
            base.ReloadData();
        }
        public void ReloadDataWithoutChangingRowStates()
        {
            if (this.treeViewDataSource == null)
            {
                mainData = new List<CITreeViewNode>();
                return;
            }
            if (treeViewDataSource.GetData().Length > treeController.TreeViewNodes.Count)
            {
                mainData = new List<CITreeViewNode>();
                treeController.TreeViewNodes.Clear();
                foreach (var item in treeViewDataSource.GetData())
                {
                    treeController.AddTreeViewNode(item);
                }
                mainData = treeController.TreeViewNodes;
            }
            base.ReloadData();
        }
        public void DeleteRows()
        {
            if (treeController.IndexPaths.Count > 0)
            {
                this.BeginUpdates();
                this.DeleteRows(treeController.IndexPaths.ToArray(), UITableViewRowAnimation.Automatic);
                this.EndUpdates();
            }
        }
        private void InsertRows()
        {
            if (treeController.IndexPaths.Count > 0)
            {
                this.BeginUpdates();
                this.InsertRows(treeController.IndexPaths.ToArray(), UITableViewRowAnimation.Automatic);
                this.EndUpdates();
            }
        }
        private void CollapseRows(CITreeViewNode treeViewNode, NSIndexPath indexPath, UIKit.UICompletionHandler completion)
        {
            if (this.treeViewDelegate == null)
                return;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                this.PerformBatchUpdates(() => { DeleteRows(); }, (bool complete) => { treeViewDelegate.DidCollapse(treeViewNode, indexPath); completion(complete); });
            }
            else
            {
                CATransaction.Begin();
                CATransaction.CompletionBlock = () => {
                    treeViewDelegate.DidCollapse(treeViewNode, indexPath);
                    completion(true);
                };
                DeleteRows();
                CATransaction.Commit();
            }
        }
        private void ExpandRows(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {
            if (treeViewDelegate == null)
                return;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                this.PerformBatchUpdates(() => { InsertRows(); }, (bool complete) => { treeViewDelegate.DidExpand(treeViewNode, indexPath); });
            }
            else
            {
                CATransaction.Begin();
                CATransaction.CompletionBlock = () => {
                    treeViewDelegate.DidCollapse(treeViewNode, indexPath);
                };
                InsertRows();
                CATransaction.Commit();
            }
        }
        UITableViewCell[] GetAllCells()
        {
            var cells = new List<UITableViewCell>();
            for (int i = 0; i < this.NumberOfSections(); i++)
            {
                for (int j = 0; j < this.NumberOfRowsInSection(i); j++)
                {
                    cells.Add(GetCell(this, NSIndexPath.FromRowSection(j, i)));
                }
            }
            return cells.ToArray();
        }
        public void ExpandAllRows()
        {
            treeController.ExpandAllRows();
            ReloadDataWithoutChangingRowStates();
        }
        public void CollapseAllRows()
        {
            treeController.CollapseAllRows();
            ReloadDataWithoutChangingRowStates();
        }
        #region UITableViewDelegate Methods
        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var treeViewNode = treeController.GetTreeViewNodeAtIndex(indexPath.Row);
            return this.treeViewDelegate.GetHeightForRow(tableView as CITreeView, indexPath, treeViewNode);
        }
        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedTreeViewNode = treeController.GetTreeViewNodeAtIndex(indexPath.Row);
            if (this.treeViewDelegate == null)
            {
                return;
            }
            if (selectedTreeViewNode != null)
            {
                treeViewDelegate.RowSelected(tableView as CITreeView, selectedTreeViewNode, indexPath);
                NSIndexPath willExpandIndexPath = indexPath;
                if (selectedTreeViewNode.Expand)
                {
                    treeController.CollapseRows(selectedTreeViewNode, indexPath);
                    CollapseRows(selectedTreeViewNode, indexPath,(bool finished) => { });
                }
                else
                {
                    if (CollapseNoneSelectedRows && selectedTreeViewNode.Level == 0)
                    {
                        var collapsedTreeViewNode = treeController.CollapseAllRowsExceptOne();
                        if (treeController.IndexPaths.Count > 0)
                        {
                            CollapseRows(collapsedTreeViewNode, indexPath, (bool complete) => {
                                willExpandIndexPath = NSIndexPath.FromRowSection(this.mainData.IndexOf(selectedTreeViewNode), indexPath.Section);
                            });
                            treeController.ExpandRows(willExpandIndexPath, selectedTreeViewNode, false);
                            ExpandRows(selectedTreeViewNode, indexPath);
                        }
                    }
                    else
                    {
                        treeController.ExpandRows(willExpandIndexPath, selectedTreeViewNode, false);
                        ExpandRows(selectedTreeViewNode, indexPath);
                    }
                }
            }

        }
        #endregion
        #region UITablewViewDataSource Methods
        public nint RowsInSection(UITableView tableView, nint section)
        {
            return treeController.TreeViewNodes.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var treeViewNode = treeController.GetTreeViewNodeAtIndex(indexPath.Row);
            return treeViewDataSource.GetCell(tableView as CITreeView, indexPath, treeViewNode);
        }
        #endregion
        #region ITreeControllerDelegate Methods
        public CITreeViewData[] GetChildren(CITreeViewData item, NSIndexPath indexPath)
        {
            return treeViewDataSource.GetChildrenForNode(item);
        }

        public void WillExpand(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {
            treeViewDelegate.WillExpand(treeViewNode, indexPath);
        }

        public void WillCollapse(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {
            treeViewDelegate.WillCollapse(treeViewNode, indexPath);
        }
        #endregion
    }
}