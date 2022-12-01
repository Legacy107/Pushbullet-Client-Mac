using AppKit;
using CoreFoundation;
using Foundation;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Darwin;
using CoreGraphics;

namespace PushBulletClient
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        NSStatusItem statusItem;
        ClientWebSocket webSocket;
        private string apiKey;

        public AppDelegate ()
        {
        }

        private async Task InitWebSocket()
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new System.Uri($"wss://stream.pushbullet.com/websocket/{apiKey}"), CancellationToken.None);
        }


        public override async void DidFinishLaunching(NSNotification notification)
        {
            NSStatusBar statusBar = NSStatusBar.SystemStatusBar;

            statusItem = statusBar.CreateStatusItem(NSStatusItemLength.Variable);
            statusItem.Title = "Text";
            statusItem.HighlightMode = true;
            statusItem.Menu = MainMenu;
            TrayIcon.Image.Template = true;
            statusItem.Button.Image = TrayIcon.Image;
            Item.View = StackNoti;

            getApiKey();

            await InitWebSocket();
            var buffer = new byte[8192];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
                else
                {
                    JObject data = JObject.Parse(Encoding.ASCII.GetString(buffer, 0, result.Count));
                    if (
                        (string)data["type"] == "push" &&
                        (string)data["push"]["type"] == "mirror"
                    )
                    {
                        string message = $"{data["push"]["application_name"]}: " +
                            $"{data["push"]["title"]}\n" +
                            $"{data["push"]["body"]} " +
                            $"{data["push"]["dismissible"]}";
                        var notiCard = new NSTextField();
                        notiCard.StringValue = message;
                        notiCard.LineBreakMode = NSLineBreakMode.ByWordWrapping;
                        StackNoti.AddArrangedSubview(notiCard);

                        // Trigger a local notification after the time has elapsed
                        var noti = new NSUserNotification();

                        // Add text and sound to the notification
                        noti.Title = (string)data["push"]["application_name"];
                        noti.Subtitle = (string)data["push"]["title"];
                        noti.InformativeText = (string)data["push"]["body"];
                        noti.SoundName = NSUserNotification.NSUserNotificationDefaultSoundName;
                        noti.HasActionButton = true;
                        if ((bool)data["push"]["dismissible"])
                        {
                            noti.ActionButtonTitle = "Dismiss";
                        }
                        NSUserNotificationCenter.DefaultUserNotificationCenter.DeliverNotification(noti);
                    }
                }
            }
        }

        private void getApiKey()
        {
            var apiKeyInput = new NSTextField(new CGRect(0, 0, 300, 20));
            var alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Informational,
                MessageText = "PushBullet API key",
            };
            alert.AddButton("Ok");
            alert.AddButton("Cancel");
            alert.AccessoryView = apiKeyInput;
            alert.Layout();
            var response = alert.RunModal();

            if (response == (int)NSAlertButtonReturn.Second)
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            apiKey = apiKeyInput.StringValue;
        }

        public override void WillTerminate (NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}

