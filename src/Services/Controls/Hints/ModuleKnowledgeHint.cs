using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Services.Models;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class ModuleKnowledgeHint : BaseHint {

        private ModuleKnowledge _knowledge;

        private BitmapFont _font;
        private BitmapFont _bigFont;
        private BitmapFont _sourceFont;

        private const string SOURCE_BIND = "— ";

        public ModuleKnowledgeHint(ModuleKnowledge knowledge) {
            _font       = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular);
            _bigFont    = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular);
            _sourceFont = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size16, ContentService.FontStyle.Italic);
            _knowledge  = knowledge;

            FadeOutDuration = _knowledge.Text.Length / 50f;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            base.Paint(spriteBatch, bounds); // Draw background

            var    center      = new Point(bounds.Width / 2, bounds.Height / 2);
            int    centerRight = center.X + center.X / 2;

            string title       = _knowledge.ModuleName ?? string.Empty;
            int    titleHeight = (int)_bigFont.MeasureString(title).Height;
            int    titleWidth  = (int)_bigFont.MeasureString(title).Width;
            var    titleCenter = new Point(center.X - titleWidth / 2, center.Y - titleHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, title, _bigFont, new Rectangle(titleCenter.X, BaseHint.TOP_PADDING, titleWidth, titleHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);

            int    srcBindWidth  = (int)_sourceFont.MeasureString(SOURCE_BIND).Width;
            int    srcBindHeight = (int)_sourceFont.MeasureString(SOURCE_BIND).Height;

            string source = DrawUtil.WrapText(_sourceFont, _knowledge.Author ?? string.Empty, centerRight / 2f);
            int srcHeight = (int)_sourceFont.MeasureString(source).Height;

            string wrappedTip = DrawUtil.WrapText(_font, _knowledge.Text ?? string.Empty, bounds.Width - BaseHint.RIGHT_PADDING);
            int    tipHeight  = (int) _font.MeasureString(wrappedTip).Height + srcHeight;
            int    tipWidth   = (int)_font.MeasureString(wrappedTip).Width;
            var    tipCenter  = new Point(center.X - tipWidth / 2, center.Y - tipHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, wrappedTip, _font, new Rectangle(tipCenter.X, tipCenter.Y, tipWidth, tipHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);

            if (!string.IsNullOrWhiteSpace(_knowledge.Author)) {
                int srcWidth = (int)_sourceFont.MeasureString(source).Width;

                int srcPaddingY     = tipCenter.Y + tipHeight / 2 + _font.LineHeight;
                int srcBindPaddingX = centerRight - srcWidth  / 2 - srcBindWidth;
                int srcPaddingX     = centerRight - srcWidth  / 2;

                spriteBatch.DrawStringOnCtrl(this, SOURCE_BIND, _sourceFont, new Rectangle(srcBindPaddingX, srcPaddingY, srcBindWidth, srcBindHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);
                spriteBatch.DrawStringOnCtrl(this, source, _sourceFont, new Rectangle(srcPaddingX, srcPaddingY, srcWidth, srcHeight), Color.White, false, true, 2, HorizontalAlignment.Left,   VerticalAlignment.Top);
            }
        }
    }
}
