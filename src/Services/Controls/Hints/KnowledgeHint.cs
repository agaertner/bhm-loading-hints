﻿using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Properties;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class KnowledgeHint : BaseHint {

        private string _knowledge;

        private BitmapFont _font;
        private BitmapFont _bigFont;

        public KnowledgeHint(string knowledge) {
            _font       = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular);
            _bigFont    = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular);
            _knowledge  = knowledge;

            FadeOutDuration = _knowledge.Length / 50f;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            base.Paint(spriteBatch, bounds); // Draw background

            var    center      = new Point(bounds.Width / 2, bounds.Height / 2);
            string title       = LoadingScreenHintsModule.Instance.Resources.GetString("Did You Know:");
            int    titleHeight = (int)_bigFont.MeasureString(title).Height;
            int    titleWidth  = (int)_bigFont.MeasureString(title).Width;
            var    titleCenter = new Point(center.X - titleWidth / 2, center.Y - titleHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, title, _bigFont, new Rectangle(titleCenter.X, BaseHint.TOP_PADDING, titleWidth, titleHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);
            string wrappedTip = DrawUtil.WrapText(_font, _knowledge ?? string.Empty, bounds.Width - BaseHint.RIGHT_PADDING);
            int    tipHeight  = (int)_font.MeasureString(wrappedTip).Height;
            int    tipWidth   = (int)_font.MeasureString(wrappedTip).Width;
            var    tipCenter  = new Point(center.X - tipWidth / 2, center.Y - tipHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, wrappedTip, _font, new Rectangle(tipCenter.X, tipCenter.Y, tipWidth, tipHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);
        }
    }
}
