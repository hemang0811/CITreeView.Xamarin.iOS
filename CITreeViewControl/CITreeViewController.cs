using System;
using System.Collections.Generic;
using Foundation;

namespace CITreeViewControl
{
    public interface ITreeControllerDelegate
    {
        CITreeViewData[] GetChildren(CITreeViewData item, NSIndexPath indexPath);
        void WillExpand(CITreeViewNode treeViewNode, NSIndexPath indexPath);
        void WillCollapse(CITreeViewNode treeViewNode, NSIndexPath indexPath);
    }

    public class CITreeController
    {
        [Weak]
        private ITreeControllerDelegate m_Delegate;
        public List<CITreeViewNode> TreeViewNodes { get; set; }
        public List<NSIndexPath> IndexPaths;

        public ITreeControllerDelegate Delegate { get => m_Delegate; set => m_Delegate = value; }

        public CITreeController(List<CITreeViewNode> treeViewNodes)
        {
            TreeViewNodes = treeViewNodes;
        }
        #region TreeViewNode Methods
        public void AddTreeViewNode(CITreeViewData item)
        {
            TreeViewNodes.Add(new CITreeViewNode(item));
        }
        public CITreeViewNode GetTreeViewNodeAtIndex(int index)
        {
            if (index < TreeViewNodes.Count)
                return TreeViewNodes[index];
            return TreeViewNodes[TreeViewNodes.Count - 1];
        }
        public int IndexOf(CITreeViewNode treeViewNode)
        {
            return TreeViewNodes.IndexOf(treeViewNode);
        }
        public NSIndexPath GetIndexPath(CITreeViewNode treeViewNode)
        {
            var index = TreeViewNodes.FindIndex(p => p == treeViewNode);
            if (index != -1)
            {
                return NSIndexPath.FromRowSection(index, 0);
            }
            return NSIndexPath.FromRowSection(0, 0);
        }
        public void InsertTreeViewNode(CITreeViewNode parent, CITreeViewData item, int index)
        {
            CITreeViewNode node = new CITreeViewNode(item);
            node.ParentNode = parent;
            TreeViewNodes.Insert(index, node);
        }
        public void RemoteTreeViewNodes(int start, int end)
        {
            TreeViewNodes.RemoveRange(start, end - start + 1);
        }
        public void SetExpandTreeViewNode(int index)
        {
            TreeViewNodes[index].Expand = true;
        }
        public void SetCollapseTreeViewNode(int index)
        {
            TreeViewNodes[index].Expand = false;
        }
        public void SetLevelTreeViewNode(int index, int level)
        {
            TreeViewNodes[index].Level = level + 1;
        }
        #endregion
        #region Expand Rows
        public void AddIndexPath(int row)
        {
            var indexPath = NSIndexPath.FromRowSection(row, 0);
            IndexPaths.Add(indexPath);
        }
        public void ExpandRows(NSIndexPath indexPath, CITreeViewNode selectedNode)
        {
            CITreeViewData[] children = Delegate.GetChildren(selectedNode.Item, indexPath);
            IndexPaths = new List<NSIndexPath>();
            int row = indexPath.Row + 1;

            if (children.Length > 0)
            {
                Delegate.WillExpand(selectedNode, indexPath);
                SetExpandTreeViewNode(indexPath.Row);
            }
            foreach (var item in children)
            {
                AddIndexPath(row);
                InsertTreeViewNode(selectedNode, item, row);
                row += 1;
            }
        }
        public int ExpandRows(NSIndexPath indexPath, CITreeViewNode selectedNode, bool openWithChildren)
        {
            if (Delegate == null)
                return 0;

            var children = Delegate.GetChildren(selectedNode.Item, indexPath);
            IndexPaths = new List<NSIndexPath>();
            var row = indexPath.Row + 1;
            SetExpandTreeViewNode(indexPath.Row);

            if (children.Length > 0)
            {
                Delegate.WillExpand(selectedNode, indexPath);
                foreach (var item in children)
                {
                    AddIndexPath(row);
                    InsertTreeViewNode(selectedNode, item, row);
                    SetLevelTreeViewNode(row, selectedNode.Level);
                    if (openWithChildren)
                    {
                        var treeNode = GetTreeViewNodeAtIndex(row);
                        var index = NSIndexPath.FromRowSection(row, 0);
                        row = ExpandRows(indexPath, treeNode, openWithChildren);
                    }
                    else
                    {
                        row += 1;
                    }
                }
            }
            return row;
        }
        #endregion
        #region Collapse Rows
        public void RemoveIndexPath(ref int row, NSIndexPath indexPath)
        {
            var treeViewNode = GetTreeViewNodeAtIndex(row);
            var children = Delegate.GetChildren(treeViewNode.Item, indexPath);
            var index = NSIndexPath.FromRowSection(row, indexPath.Section);
            IndexPaths.Add(index);
            row += 1;

            if (treeViewNode.Expand)
            {
                foreach (var child in children)
                {
                    RemoveIndexPath(ref row, indexPath);

                }
            }
        }
        public void CollapseRows(CITreeViewNode treeViewNode, NSIndexPath indexPath)
        {
            if (Delegate == null)
                return;
            var children = Delegate.GetChildren(treeViewNode.Item, indexPath);
            IndexPaths = new List<NSIndexPath>();
            var row = indexPath.Row + 1;

            if (children.Length > 0)
            {
                Delegate.WillCollapse(treeViewNode, indexPath);
            }
            SetCollapseTreeViewNode(indexPath.Row);

            foreach (var child in children)
            {
                RemoveIndexPath(ref row, indexPath);
            }
            if (IndexPaths.Count > 0)
            {
                RemoteTreeViewNodes(IndexPaths[0].Row, IndexPaths[IndexPaths.Count - 1].Row);
            }
        }
        public CITreeViewNode CollapseAllRowsExceptOne()
        {
            IndexPaths = new List<NSIndexPath>();
            CITreeViewNode collapsedNode = null;
            var indexPath = NSIndexPath.FromRowSection(0, 0);
            foreach (CITreeViewNode node in TreeViewNodes)
            {
                if (node.Expand && node.Level == 0)
                {
                    CollapseRows(node, indexPath);
                    collapsedNode = node;
                }
                indexPath = NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section);
            }
            return collapsedNode;
        }
        public void CollapseAllRows()
        {
            IndexPaths = new List<NSIndexPath>();
            var indexPath = NSIndexPath.FromRowSection(0, 0);

            foreach (CITreeViewNode node in TreeViewNodes)
            {
                indexPath = GetIndexPath(node);
                if (node.Level != 0)
                {
                    SetCollapseTreeViewNode(indexPath.Row);
                    TreeViewNodes.RemoveAt(indexPath.Row);
                }
                else
                {
                    SetCollapseTreeViewNode(indexPath.Row);
                }
            }

        }
        public void ExpandAllRows()
        {
            IndexPaths = new List<NSIndexPath>();
            var indexPath = NSIndexPath.FromRowSection(0, 0);
            foreach (var node in TreeViewNodes)
            {
                if (!node.Expand)
                {
                    indexPath = GetIndexPath(node);
                    int row = ExpandRows(indexPath, node, true);
                    indexPath = NSIndexPath.FromRowSection(row, indexPath.Section);
                }
            }
        }
        #endregion
    }
}
