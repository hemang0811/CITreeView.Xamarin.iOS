using System;
using System.Collections.Generic;
using UIKit;
using Foundation;

namespace CITreeViewControl
{
    public partial class ViewController : UIViewController, ITreeViewDelegate, ITreeViewDataSource
    {
        List<CITreeViewData> data = new List<CITreeViewData>();

        string treeViewCellIdentifier = CITreeViewCell.Key;
        const string treeViewCellNibName = "CITreeViewCell";

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            data = CITreeViewData.GetDefaultData();
            sampleTreeView.CollapseNoneSelectedRows = false;
            sampleTreeView.RegisterNibForCellReuse(CITreeViewCell.Nib,CITreeViewCell.Key);
            sampleTreeView.treeViewDelegate = this;
            sampleTreeView.treeViewDataSource = this;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public void WillExpand(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {

        }

        public void WillCollapse(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {

        }

        public UITableViewCell GetCell(CITreeView treeView, NSIndexPath indexPath, CITreeViewNode treeViewNode)
        {
            var cell = treeView.DequeueReusableCell(treeViewCellIdentifier) as CITreeViewCell;

            var dataObj = treeViewNode.Item as CITreeViewData;
            cell.SetupCell(treeViewNode.Level, dataObj.Name);
            return cell;
        }

        public CITreeViewData[] GetChildrenForNode(CITreeViewData node)
        {
            return node.Children.ToArray();
        }

        public CITreeViewData[] GetData()
        {
            return data.ToArray();
        }

        public nfloat GetHeightForRow(CITreeView treeView, NSIndexPath indexPath, CITreeViewNode treeViewNode)
        {
            return 60;
        }

        public void RowSelected(CITreeView treeView, CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {

        }

        public void RowDeSelected(CITreeView treeView, CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {
            if (treeViewNode.ParentNode != null)
                Console.WriteLine(treeViewNode.ParentNode.Item);
        }

        public void DidExpand(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {

        }

        public void DidCollapse(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {

        }
    }
}
