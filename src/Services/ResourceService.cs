using Blish_HUD;
using Flurl.Http;
using Microsoft.Xna.Framework.Graphics;
using Nekres.Loading_Screen_Hints.Services.Controls.Hints;
using Nekres.Loading_Screen_Hints.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            var knowledgeUrl       = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-knowledge.json";
            var moduleKnowledgeUrl = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-modules.json";
            var quotesUrl          = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-quotes.json";
            var charactersUrl      = $"{_baseUrl}{locale.TwoLetterISOLanguageName}-characters.json";

            _knowledge       = await HttpUtil.RetryAsync(() => knowledgeUrl.GetJsonAsync<List<string>>())                                   ?? _knowledge;
            _quotes          = await HttpUtil.RetryAsync(() => quotesUrl.GetJsonAsync<List<Quote>>())                                       ?? _quotes;
            _characters      = await HttpUtil.RetryAsync(() => charactersUrl.GetJsonAsync<List<CharacterRiddle>>())                         ?? _characters;
            _moduleKnowledge = FilterModuleHints(await HttpUtil.RetryAsync(() => moduleKnowledgeUrl.GetJsonAsync<List<ModuleKnowledge>>())) ?? _moduleKnowledge;
        }

        public async Task<BaseHint> NextHint() {
            var totalHints = _knowledge.Count + _moduleKnowledge.Count + _quotes.Count + _characters.Count;

            if (totalHints <= 0) {
                return null;
            }

            int randomValue = RandomUtil.GetRandom(1, totalHints);

            int currentCount = 0;

            // Select hint type with chance based on amount of each type (like selecting from a single joined list).
            if (_knowledge.Any()) {
                currentCount += _knowledge.Count;
                if (randomValue <= currentCount) {
                    return new KnowledgeHint(_knowledge[RandomUtil.GetRandom(0, _knowledge.Count - 1)]);
                }
            }

            if (_moduleKnowledge.Any()) {
                currentCount += _moduleKnowledge.Count;
                if (randomValue <= currentCount) {
                    return new ModuleKnowledgeHint(_moduleKnowledge[RandomUtil.GetRandom(0, _moduleKnowledge.Count - 1)]);
                }
            }

            if (_quotes.Any()) {
                currentCount += _quotes.Count;
                if (randomValue <= currentCount) {
                    return new QuoteHint(_quotes[RandomUtil.GetRandom(0, _quotes.Count - 1)]);
                }
            }

            if (_characters.Any()) {
                currentCount += _characters.Count;
                if (randomValue <= currentCount) {
                    var characterHint = _characters[RandomUtil.GetRandom(0, _characters.Count - 1)];
                    var imageBytes    = await HttpUtil.TryAsync(() => $"{_baseUrl}characters/{characterHint.Image}".GetBytesAsync());

                    if (imageBytes == null) {
                        return null;
                    }

                    try {
                        using var textureStream = new MemoryStream(imageBytes);
                        var       loadedTexture = Texture2D.FromStream(GameService.Graphics.GraphicsDeviceManager.GraphicsDevice, textureStream);
                        characterHint.Texture.SwapTexture(loadedTexture);
                    } catch (Exception ex) {
                        LoadingScreenHintsModule.Logger.Debug(ex, ex.Message);
                    }

                    return new CharacterRiddleHint(characterHint);
                }
            }

            return null;
        }

        private List<ModuleKnowledge> FilterModuleHints(List<ModuleKnowledge> moduleHints) {
            if (moduleHints == null) {
                return null;
            }
            var result = new List<ModuleKnowledge>();
            foreach (var moduleKnowledge in moduleHints) {
                var l      = GameService.Module.Modules;
                var module = GameService.Module.Modules.FirstOrDefault(mm => mm.Manifest.Namespace.StartsWith(moduleKnowledge.ManifestNamespace, StringComparison.InvariantCultureIgnoreCase));
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
