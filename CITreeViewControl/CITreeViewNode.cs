using System;
namespace CITreeViewControl
{
    public sealed class CITreeViewNode
    {
        public CITreeViewNode ParentNode { get; set; }
        public bool Expand = false;
        public int Level = 0;
        public CITreeViewData Item { get; set; }

        public CITreeViewNode(CITreeViewData value)
        {
            this.Item = value;
        }
    }
}
