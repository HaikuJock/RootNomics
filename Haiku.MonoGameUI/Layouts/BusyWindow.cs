using Haiku.Audio;
using Microsoft.Xna.Framework;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class BusyWindow : Window
    {
        readonly BusyIndicator busyIndicator;
        readonly Label messageLabel;
        readonly BusyListening listener;

        public BusyWindow(Rectangle frame, SpriteFrame sprite, AudioPlaying audio, string message = "", BusyListening listener = null) 
            : base(frame, audio)
        {
            this.listener = listener;
            var background = new Panel(frame.Width, frame.Height)
            {
                BackgroundColor = Color.Black * 0.5f
            };
            busyIndicator = new BusyIndicator(BusyIndicator.LargeSize, sprite);
            background.AddChild(busyIndicator);
            busyIndicator.CenterXInParent();
            busyIndicator.CenterYInParent();
            busyIndicator.Frame = new Rectangle(
                busyIndicator.Frame.X + BusyIndicator.LargeSize / 2,
                busyIndicator.Frame.Y,
                busyIndicator.Frame.Width,
                busyIndicator.Frame.Height);
            messageLabel = new Label(message)
            {
                Font = TitleFont,
                TextColor = Color.White,
                ShadowColor = Color.Black,
                ShadowOffset = new Point(2, 2)
            };
            background.AddChild(messageLabel);
            UpdateMessage(message);

            AddChild(background);
        }

        public void UpdateMessage(string message)
        {
            messageLabel.Text = message;
            messageLabel.SizeToFit();
            messageLabel.CenterXInParent();
            messageLabel.Frame = new Rectangle(
                messageLabel.Frame.X,
                busyIndicator.Frame.Bottom,
                messageLabel.Frame.Width,
                messageLabel.Frame.Height);
        }

        public override void OnAppear()
        {
            busyIndicator.StartAnimating();
        }

        public override void OnPopped()
        {
            listener?.OnBusyPopped();
            base.OnPopped();
        }
    }
}
