using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Properties;
using Nekres.Loading_Screen_Hints.Services.Models;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class CharacterRiddleHint : BaseHint {

        private CharacterRiddle _characterRiddle;

        private BitmapFont            _font;
        private Effect                _silhouetteFX;
        private Effect                _glowFx;
        private SpriteBatchParameters _defaultParams;
        private SpriteBatchParameters _effectParams;
        private bool                  _disposed;

        public CharacterRiddleHint(CharacterRiddle characterRiddle) {
            _characterRiddle = characterRiddle;
            _font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular);
            _silhouetteFX        = GameService.Content.ContentManager.Load<Effect>(@"effects\silhouette");
            _glowFx              = GameService.Content.ContentManager.Load<Effect>(@"effects\glow");

            _silhouetteFX.Parameters["GlowColor"].SetValue(Color.White.ToVector4());
            _glowFx.Parameters["GlowColor"].SetValue(Color.White.ToVector4());
            
            _defaultParams = new SpriteBatchParameters();
            _effectParams = new SpriteBatchParameters {
                Effect = _silhouetteFX,
                BlendState = BlendState.NonPremultiplied
            };

            FadeOutDuration = 3f;
        }

        protected override void DisposeControl() {
            _disposed = true;
            _glowFx?.Dispose();
            _silhouetteFX?.Dispose();
            base.DisposeControl();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (_characterRiddle.Texture is not {HasSwapped: true} || _disposed) {
                return;
            }

            base.Paint(spriteBatch, bounds); // Draw background

            var texture = _characterRiddle.Texture;

            string title;
            if (GameService.GameIntegration.Gw2Instance.IsInGame) { // Show solution or not
                _effectParams.Effect = _glowFx;
                _glowFx.Parameters["Opacity"].SetValue(_opacity);
                _glowFx.Parameters["TextureWidth"].SetValue((float)texture.Width);
                title = string.Format(Resources.It_s__0__, _characterRiddle.Name ?? string.Empty);
            } else {
                _effectParams.Effect = _silhouetteFX;
                _silhouetteFX.Parameters["Opacity"].SetValue(_opacity);
                _silhouetteFX.Parameters["TextureWidth"].SetValue((float)texture.Width);
                title = Resources.Who_s_that_Character_;
            }

            var center = new Point(bounds.Width / 2, bounds.Height / 2);
            int centerLeft = center.X / 2;
            int centerRight = center.X + center.X / 2;

            var imgSize = PointExtensions.ResizeKeepAspect(new Point(texture.Width, texture.Height), bounds.Width / 2 - BaseHint.RIGHT_PADDING, bounds.Height - BaseHint.TOP_PADDING);

            var imgCenter = new Point(centerLeft - imgSize.X / 2, center.Y - imgSize.Y / 2);

            spriteBatch.End();
            spriteBatch.Begin(_effectParams);
            spriteBatch.DrawOnCtrl(this, texture, new Rectangle(imgCenter.X, imgCenter.Y, imgSize.X, imgSize.Y), Color.White);
            spriteBatch.End();
            spriteBatch.Begin(_defaultParams);

            string wrappedTitle = DrawUtil.WrapText(_font, title, bounds.Width / 2 - BaseHint.RIGHT_PADDING);
            int    titleHeight  = (int)_font.MeasureString(wrappedTitle).Height;
            int    titleWidth   = (int)_font.MeasureString(wrappedTitle).Width;
            var    titleCenter  = new Point(centerRight - titleWidth / 2, center.Y - titleHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, wrappedTitle, _font, new Rectangle(titleCenter.X, titleCenter.Y, bounds.Width, bounds.Height), Color.White, false, true, 2, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
    }
}
