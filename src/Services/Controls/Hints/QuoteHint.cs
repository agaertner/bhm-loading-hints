using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Services.Models;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class QuoteHint : BaseHint {

        private Quotation quotation;

        private BitmapFont _font;
        private BitmapFont _sourceFont;

        private const string QUOTATION  = "“{0}”";
        private const string SOURCE_BIND = "— ";

        public QuoteHint(Quotation quotation) {
            _font          = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular);
            _sourceFont    = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size16, ContentService.FontStyle.Italic);
            this.quotation = quotation;

            FadeOutDuration = quotation.Text.Length / 50f;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            base.Paint(spriteBatch, bounds); // Draw background

            var center = new Point(bounds.Width / 2, bounds.Height / 2);
            int centerRight = center.X + center.X / 2;

            string citation      = DrawUtil.WrapText(_font, string.Format(QUOTATION, quotation.Text ?? string.Empty), bounds.Width - BaseHint.RIGHT_PADDING);
            int    srcBindWidth  = (int)_sourceFont.MeasureString(SOURCE_BIND).Width;
            int    srcBindHeight = (int)_sourceFont.MeasureString(SOURCE_BIND).Height;

            string source    = DrawUtil.WrapText(_sourceFont, quotation.Source ?? string.Empty, centerRight / 2f);
            int    srcHeight = (int)_sourceFont.MeasureString(source).Height;
            int    srcWidth  = (int)_sourceFont.MeasureString(source).Width;
            //var srcCenter = new Point(center.X - srcWidth / 2, center.Y - srcHeight / 2);

            int textHeight = (int)_font.MeasureString(citation).Height + srcHeight;
            int textWidth  = (int)_font.MeasureString(citation).Width;
            var textCenter = new Point(center.X - textWidth / 2, center.Y - textHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, citation, _font, new Rectangle(textCenter.X, textCenter.Y, textWidth, textHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);

            int srcPaddingY     = textCenter.Y + textHeight / 2 + _font.LineHeight;
            int srcBindPaddingX = centerRight  - srcWidth   / 2 - srcBindWidth;
            spriteBatch.DrawStringOnCtrl(this, SOURCE_BIND, _sourceFont, new Rectangle(srcBindPaddingX, srcPaddingY, srcBindWidth, srcBindHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);

            int srcPaddingX = centerRight - srcWidth / 2;
            spriteBatch.DrawStringOnCtrl(this, source, _sourceFont, new Rectangle(srcPaddingX, srcPaddingY, srcWidth, srcHeight), Color.White, false, true, 2, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
    }
}
