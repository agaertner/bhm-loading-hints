using Blish_HUD;
using Flurl.Http;
using Nekres.Loading_Screen_Hints.Services.Controls.Hints;
using Nekres.Loading_Screen_Hints.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Nekres.Loading_Screen_Hints.Services {
    internal class ResourceService : IDisposable {
        private string _baseUrl = "https://raw.githubusercontent.com/agaertner/bhm-loading-hints/main/hints/";

        private List<string>          _knowledge;
        private List<ModuleKnowledge> _moduleKnowledge;
        private List<Quote>           _quotes;
        private List<CharacterRiddle> _characters;

        public ResourceService() {
            GameService.Overlay.UserLocaleChanged += OnUserLocaleChanged;

            _knowledge       = new List<string>();
            _moduleKnowledge = new List<ModuleKnowledge>();
            _quotes          = new List<Quote>();
            _characters      = new List<CharacterRiddle>();
        }

        public async Task LoadAsync(CultureInfo locale) {
            var knowledgeUrl       = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-knowledge";
            var moduleKnowledgeUrl = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-modules";
            var quotesUrl          = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-quotes";
            var charactersUrl      = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-characters";

            _knowledge       = await HttpUtil.RetryAsync(() => knowledgeUrl.GetJsonAsync<List<string>>())                                   ?? _knowledge;
            _quotes          = await HttpUtil.RetryAsync(() => quotesUrl.GetJsonAsync<List<Quote>>())                                       ?? _quotes;
            _characters      = await HttpUtil.RetryAsync(() => charactersUrl.GetJsonAsync<List<CharacterRiddle>>())                         ?? _characters;
            _moduleKnowledge = FilterModuleHints(await HttpUtil.RetryAsync(() => moduleKnowledgeUrl.GetJsonAsync<List<ModuleKnowledge>>())) ?? _moduleKnowledge;
        }

        public BaseHint NextHint() {
            var maxCount = Math.Max(Math.Max(_knowledge.Count, _moduleKnowledge.Count), Math.Max(_quotes.Count, _characters.Count));

            int randomValue = RandomUtil.GetRandom(0, maxCount + 1);

            int currentCount = 0;

            currentCount += _knowledge.Count;
            if (randomValue <= currentCount) {
                return new KnowledgeHint(_knowledge[randomValue]);
            }

            currentCount += _moduleKnowledge.Count;
            if (randomValue <= currentCount) {
                return new ModuleKnowledgeHint(_moduleKnowledge[randomValue]);
            }

            currentCount += _quotes.Count;
            if (randomValue <= currentCount) {
                return new QuoteHint(_quotes[randomValue]);
            }

            currentCount += _characters.Count;
            if (randomValue <= currentCount) {
                return new CharacterRiddleHint(_characters[randomValue]);
            }

            return null;
        }

        private List<ModuleKnowledge> FilterModuleHints(List<ModuleKnowledge> moduleHints) {
            if (moduleHints == null) {
                return null;
            }
            var result = new List<ModuleKnowledge>();
            foreach (var moduleKnowledge in moduleHints) {
                var module = GameService.Module.Modules.FirstOrDefault(mm => mm.Manifest.Namespace.StartsWith(moduleKnowledge.ManifestNamespace));
                if (module is {Enabled: true}) { // Filter hints for enabled modules.
                    result.Add(moduleKnowledge);
                }
            }
            return result;
        }

        private async void OnUserLocaleChanged(object sender, ValueEventArgs<CultureInfo> e) {
            await LoadAsync(e.Value);
        }

        public void Dispose() {
            GameService.Overlay.UserLocaleChanged -= OnUserLocaleChanged;
        }

    }
}
