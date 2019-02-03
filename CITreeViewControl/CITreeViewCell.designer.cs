// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CITreeViewControl
{
    [Register ("CITreeViewCell")]
    partial class CITreeViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView avatarImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint leadingConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (avatarImageView != null) {
                avatarImageView.Dispose ();
                avatarImageView = null;
            }

            if (leadingConstraint != null) {
                leadingConstraint.Dispose ();
                leadingConstraint = null;
            }

            if (nameLabel != null) {
                nameLabel.Dispose ();
                nameLabel = null;
            }
        }
    }
}