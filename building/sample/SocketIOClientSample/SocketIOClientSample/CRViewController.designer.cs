// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SocketIOClientSample
{
	[Register ("CRViewController")]
	partial class CRViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView loginPage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView messageArea { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField messageField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField nickName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton sendBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel typingLabel { get; set; }

		[Action ("onSend:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void onSend (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (loginPage != null) {
				loginPage.Dispose ();
				loginPage = null;
			}
			if (messageArea != null) {
				messageArea.Dispose ();
				messageArea = null;
			}
			if (messageField != null) {
				messageField.Dispose ();
				messageField = null;
			}
			if (nickName != null) {
				nickName.Dispose ();
				nickName = null;
			}
			if (sendBtn != null) {
				sendBtn.Dispose ();
				sendBtn = null;
			}
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}
			if (typingLabel != null) {
				typingLabel.Dispose ();
				typingLabel = null;
			}
		}
	}
}
