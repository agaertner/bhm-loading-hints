using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nekres.Loading_Screen_Hints.Properties;
using System;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public abstract class BaseHint : Control {

        public const int TOP_PADDING = 20;
        public const int RIGHT_PADDING = 40;

        private Texture2D _bgTex;

        protected float FadeOutDuration;

        protected BaseHint() {
            BasicTooltipText = string.Format(Resources.Click_to__0_, Resources.Hide);
            Parent = GameService.Graphics.SpriteScreen;
            Size   = new Point(600, 200);

            _bgTex = LoadingScreenHintsModule.Instance.ContentsManager.GetTexture("background_loadscreenpanel.png");

            Center(Parent.AbsoluteBounds);
            Graphics.SpriteScreen.ContentResized += UpdateLocation;
        }

        private Glide.Tween _fadeOut;
        private Glide.Tween _toggleFade;
        private bool        _fadeDirection;

        public void FadeOut() {
            this.BasicTooltipText = string.Empty;

            // Already invisible.
            if (_opacity <= 0) {
                this.Dispose();
                return;
            }

            _toggleFade?.Cancel();
            _fadeOut = GameService.Animation.Tweener
                                  .Tween(this, new { Opacity = 0.0f }, 2f + FadeOutDuration)
                                  .OnComplete(Dispose);
        }

        protected override void OnLeftMouseButtonReleased(MouseEventArgs e) {
            Toggle();
        }

        protected override void OnRightMouseButtonReleased(MouseEventArgs e) {
            Toggle();
        }

        private void Toggle() {
            if (_fadeOut is { Completion: > 0 }) {
                return;
            }

            _toggleFade?.Cancel();
            _toggleFade    = GameService.Animation.Tweener.Tween(this, new { Opacity = (float)Convert.ToInt32(_fadeDirection) }, 0.2f);
            _fadeDirection = !_fadeDirection;

            this.BasicTooltipText = string.Format(Resources.Click_to__0_, _fadeDirection ? Resources.Show : Resources.Hide);
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Mouse;
        }

        protected override void DisposeControl() {
            _toggleFade?.Cancel();
            _fadeOut?.Cancel();
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
            this.Location = new Point((bounds.Width - this.Width) / 2,  bounds.Height - this.Height * 2);
        }
    }

}
