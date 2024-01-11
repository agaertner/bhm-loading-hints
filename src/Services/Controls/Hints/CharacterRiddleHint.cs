using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Properties;
using Nekres.Loading_Screen_Hints.Services.Models;
using System;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class CharacterRiddleHint : BaseHint {

        private CharacterRiddle _characterRiddle;

        private BitmapFont            _font;
        private Effect                _silhouetteFx;
        private Effect                _glowFx;
        private SpriteBatchParameters _defaultParams;
        private SpriteBatchParameters _effectParams;

        public CharacterRiddleHint(CharacterRiddle characterRiddle, Effect glowFx, Effect silhouetteFx) {
            _characterRiddle = characterRiddle;
            _font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular);

            _defaultParams = new SpriteBatchParameters();
            _glowFx        = glowFx;
            _silhouetteFx  = silhouetteFx;
            _effectParams = new SpriteBatchParameters {
                Effect     = _silhouetteFx,
                BlendState = BlendState.NonPremultiplied
            };

            FadeOutDuration = 3f;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (_characterRiddle.Texture is not {HasSwapped: true}) {
                return;
            }

            base.Paint(spriteBatch, bounds); // Draw background

            var texture = _characterRiddle.Texture;

            string title;
            if (GameService.GameIntegration.Gw2Instance.IsInGame) { // Show solution or not
                _effectParams.Effect = _glowFx;
                _glowFx.Parameters["Opacity"].SetValue(_opacity);
                _glowFx.Parameters["TextureWidth"].SetValue((float)texture.Width);
                title = string.Format(LoadingScreenHintsModule.Instance.Resources.GetString("It's {0}!"), _characterRiddle.Name ?? string.Empty);
            } else {
                _effectParams.Effect = _silhouetteFx;
                _silhouetteFx.Parameters["Opacity"].SetValue(_opacity);
                _silhouetteFx.Parameters["TextureWidth"].SetValue((float)texture.Width);
                title = LoadingScreenHintsModule.Instance.Resources.GetString("Who's that Character?");
            }

            var center = new Point(bounds.Width / 2, bounds.Height / 2);
            int centerLeft = center.X / 2;
            int centerRight = center.X + center.X / 2;

            var imgSize = PointExtensions.ResizeKeepAspect(new Point(texture.Width, texture.Height), bounds.Width / 2 - BaseHint.RIGHT_PADDING, bounds.Height - BaseHint.TOP_PADDING);

            var imgCenter = new Point(centerLeft - imgSize.X / 2, center.Y - imgSize.Y / 2);

            try {
                spriteBatch.End();
                spriteBatch.Begin(_effectParams);
                spriteBatch.DrawOnCtrl(this, texture, new Rectangle(imgCenter.X, imgCenter.Y, imgSize.X, imgSize.Y), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(_defaultParams);
            } catch (ObjectDisposedException) {
                // Module was unloaded and effects were disposed while we were drawing.
                return;
            }

            string wrappedTitle = DrawUtil.WrapText(_font, title, bounds.Width / 2 - BaseHint.RIGHT_PADDING);
            int    titleHeight  = (int)_font.MeasureString(wrappedTitle).Height;
            int    titleWidth   = (int)_font.MeasureString(wrappedTitle).Width;
            var    titleCenter  = new Point(centerRight - titleWidth / 2, center.Y - titleHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, wrappedTitle, _font, new Rectangle(titleCenter.X, titleCenter.Y, bounds.Width, bounds.Height), Color.White, false, true, 2, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
    }
}
