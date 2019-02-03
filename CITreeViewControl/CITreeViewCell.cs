using System;

using Foundation;
using UIKit;

namespace CITreeViewControl
{
    public partial class CITreeViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("CITreeViewCell");
        public static readonly UINib Nib;
        private nfloat leadingValueForChildrenCell = 30.0f;

        static CITreeViewCell()
        {
            Nib = UINib.FromName("CITreeViewCell", NSBundle.MainBundle);
        }

        protected CITreeViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        public void SetupCell(int level, string text)
        {
            this.nameLabel.Text = text;
            this.leadingConstraint.Constant = leadingValueForChildrenCell * (level + 1);
            this.avatarImageView.Layer.CornerRadius = this.avatarImageView.Frame.Size.Height / 2;
            switch (level)
            {
                case 0:
                    avatarImageView.BackgroundColor = UIColor.Orange;
                    break;
                case 1:
                    avatarImageView.BackgroundColor = UIColor.Green;
                    break;
                case 2:
                    avatarImageView.BackgroundColor = UIColor.Blue;
                    break;
                default:
                    avatarImageView.BackgroundColor = UIColor.Black;
                    break;
            }
            LayoutIfNeeded();
        }
        UIColor GetRandomColor()
        {
            Random randomNumber = new Random();
            nfloat red = new nfloat(randomNumber.NextDouble());
            nfloat green = new nfloat(randomNumber.NextDouble());
            nfloat blue = new nfloat(randomNumber.NextDouble());
            return UIColor.FromRGBA(red, green, blue, 1);
        }
        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
        }
    }
}
