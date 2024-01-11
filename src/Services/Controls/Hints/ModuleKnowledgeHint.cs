using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.Loading_Screen_Hints.Properties;
using Nekres.Loading_Screen_Hints.Services.Models;

namespace Nekres.Loading_Screen_Hints.Services.Controls.Hints {
    public class ModuleKnowledgeHint : BaseHint {

        private ModuleKnowledge _knowledge;

        private BitmapFont _font;
        private BitmapFont _bigFont;

        public ModuleKnowledgeHint(ModuleKnowledge knowledge) {
            _font      = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular);
            _bigFont   = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular);
            _knowledge = knowledge;

            ReadingTime = _knowledge.Text.Length / 40f;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            base.Paint(spriteBatch, bounds); // Draw background

            var    center      = new Point(bounds.Width / 2, bounds.Height / 2);
            string title       = Resources.Did_You_Know_;
            int    titleHeight = (int)_bigFont.MeasureString(title).Height;
            int    titleWidth  = (int)_bigFont.MeasureString(title).Width;
            var    titleCenter = new Point(center.X - titleWidth / 2, center.Y - titleHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, title, _bigFont, new Rectangle(titleCenter.X, BaseHint.TOP_PADDING, titleWidth, titleHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);
            string wrappedTip = DrawUtil.WrapText(_font, _knowledge.Text, bounds.Width - BaseHint.RIGHT_PADDING);
            int    tipHeight  = (int)_font.MeasureString(wrappedTip).Height;
            int    tipWidth   = (int)_font.MeasureString(wrappedTip).Width;
            var    tipCenter  = new Point(center.X - tipWidth / 2, center.Y - tipHeight / 2);
            spriteBatch.DrawStringOnCtrl(this, wrappedTip, _font, new Rectangle(tipCenter.X, tipCenter.Y, tipWidth, tipHeight), Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Top);
        }
    }
}
