using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public abstract class BaseHint : Control {

        public const int TOP_PADDING = 20;
        public const int RIGHT_PADDING = 40;

        private Texture2D _bgTex;

        protected float ReadingTime;

        protected BaseHint() {
            Parent = GameService.Graphics.SpriteScreen;
            Size   = new Point(600, 200);

            _bgTex = LoadingScreenHintsModule.Instance.ContentsManager.GetTexture("background_loadscreenpanel.png");

            Center(Parent.AbsoluteBounds);
            Graphics.SpriteScreen.ContentResized += UpdateLocation;
        }

        public Glide.Tween Fade { get; private set; }

        public void FadeOut() {
            if (_opacity != 1.0f) {
                return;
            }

            Fade = Animation.Tweener.Tween(this, new { Opacity = 0.0f }, 2f + ReadingTime);
            Fade.OnComplete(Dispose);
        }

        protected override void OnLeftMouseButtonReleased(MouseEventArgs e) {
            if (_opacity != 1.0) {
                return;
            }

            GameService.Animation.Tweener.Tween(this, new { Opacity = 0.0f }, 0.2f);
        }

        protected override void OnRightMouseButtonReleased(MouseEventArgs e) {
            if (_opacity != 0.0f) {
                return;
            }

            GameService.Animation.Tweener.Tween(this, new { Opacity = 1.0f }, 0.2f);
            base.OnRightMouseButtonReleased(e);
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Mouse;
        }

        protected override void DisposeControl() {
            _bgTex?.Dispose();
            base.DisposeControl();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            spriteBatch.DrawOnCtrl(this, _bgTex, new Rectangle(0, 0, bounds.Width, bounds.Height), Color.White);
        }

        private void UpdateLocation(object sender, RegionChangedEventArgs e) {
            Center(e.CurrentRegion);
        }

        private void Center(Rectangle bounds) {
            this.Location = new Point((bounds.Width - this.Width) / 2, bounds.Height - this.Height);
        }
    }

}
