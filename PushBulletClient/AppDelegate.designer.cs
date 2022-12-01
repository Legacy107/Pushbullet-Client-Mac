// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PushBulletClient
{
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem Item { get; set; }

		[Outlet]
		AppKit.NSMenu MainMenu { get; set; }

		[Outlet]
		AppKit.NSStackView StackNoti { get; set; }

		[Outlet]
		AppKit.NSImageView TrayIcon { get; set; }

		[Action ("Button:")]
		partial void Button (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Item != null) {
				Item.Dispose ();
				Item = null;
			}

			if (MainMenu != null) {
				MainMenu.Dispose ();
				MainMenu = null;
			}

			if (StackNoti != null) {
				StackNoti.Dispose ();
				StackNoti = null;
			}

			if (TrayIcon != null) {
				TrayIcon.Dispose ();
				TrayIcon = null;
			}
		}
	}
}
