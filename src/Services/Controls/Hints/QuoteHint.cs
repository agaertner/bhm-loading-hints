using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Services.Models;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class QuoteHint : BaseHint {

        private Quote _quote;

        private BitmapFont _font;
        private BitmapFont _sourceFont;

        public QuoteHint(Quote quote) {
            _font       = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular);
            _sourceFont = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size16, ContentService.FontStyle.Italic);
            _quote      = quote;

            FadeOutDuration = quote.Text.Length / 40f;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            base.Paint(spriteBatch, bounds); // Draw background

            var center = new Point(bounds.Width / 2, bounds.Height / 2);
            int centerRight = center.X + center.X / 2;

            var alignCenter = HorizontalAlignment.Center;
            var alignTop = VerticalAlignment.Top;

            string citation = DrawUtil.WrapText(_font, _quote.Text, bounds.Width - BaseHint.RIGHT_PADDING);
            string sourceBind = "— ";
            int srcBindWidth = (int)_sourceFont.MeasureString(sourceBind).Width;
            int srcBindHeight = (int)_sourceFont.MeasureString(sourceBind).Height;
            string source = DrawUtil.WrapText(_sourceFont, _quote.Source, centerRight / 2f);

            int srcHeight = (int)_sourceFont.MeasureString(source).Height;
            int srcWidth  = (int)_sourceFont.MeasureString(source).Width;
            var srcCenter = new Point(center.X - srcWidth / 2, center.Y - srcHeight / 2);

            int textHeight = (int)_font.MeasureString(citation).Height + srcHeight;
            int textWidth  = (int)_font.MeasureString(citation).Width;
            var textCenter = new Point(center.X - textWidth / 2, center.Y - textHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, citation, _font, new Rectangle(textCenter.X, textCenter.Y, textWidth, textHeight), Color.White, false, true, 2, alignCenter, alignTop);

            int srcPaddingY     = textCenter.Y + textHeight / 2 + _font.LineHeight;
            int srcBindPaddingX = centerRight  - srcWidth   / 2 - srcBindWidth;
            spriteBatch.DrawStringOnCtrl(this, sourceBind, _sourceFont, new Rectangle(srcBindPaddingX, srcPaddingY, srcBindWidth, srcBindHeight), Color.White, false, true, 2, alignCenter, alignTop);

            int srcPaddingX = centerRight - srcWidth / 2;
            spriteBatch.DrawStringOnCtrl(this, source, _sourceFont, new Rectangle(srcPaddingX, srcPaddingY, srcWidth, srcHeight), Color.White, false, true, 2, HorizontalAlignment.Left, alignTop);
        }
    }
}
