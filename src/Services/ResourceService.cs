using Blish_HUD;
using Blish_HUD.Content;
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

        private List<string> _supportedLocales = new() {
            "en",
            "de",
            "es",
            "fr"
        };

        public ResourceService() {
            GameService.Overlay.UserLocaleChanged += OnUserLocaleChanged;

            _knowledge       = new List<string>();
            _moduleKnowledge = new List<ModuleKnowledge>();
            _quotes          = new List<Quote>();
            _characters      = new List<CharacterRiddle>();
        }

        public async Task LoadAsync(CultureInfo locale) {
            DisposeTextures();

            var langCode = _supportedLocales.Any(lang => lang.Equals(locale.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase)) 
                               ? locale.TwoLetterISOLanguageName.ToLowerInvariant()
                               : "en"; // Fallback to english.
            
            var knowledgeUrl       = $"{_baseUrl}{langCode}-knowledge.json";
            var moduleKnowledgeUrl = $"{_baseUrl}{langCode}-modules.json";
            var quotesUrl          = $"{_baseUrl}{langCode}-quotes.json";
            var charactersUrl      = $"{_baseUrl}{langCode}-characters.json";

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

            // Every hint seen, reset.
            if (LoadingScreenHintsModule.Instance.SeenHints.Value.Count >= totalHints) {
                LoadingScreenHintsModule.Instance.SeenHints.Value = new List<int>();
                await LoadAsync(CultureInfo.CurrentUICulture);
                return await NextHint();
            }

            if (LoadingScreenHintsModule.Instance.SeenHints.Value.Contains(randomValue)) {
                return await NextHint();
            }

            int currentCount = 0;

            // Select hint type with chance based on amount of each type (like selecting from a single joined list).
            if (_knowledge.Any()) {
                currentCount += _knowledge.Count;
                if (randomValue <= currentCount) {
                    LoadingScreenHintsModule.Instance.SeenHints.Value.Add(randomValue);
                    return new KnowledgeHint(_knowledge[randomValue - (currentCount - _knowledge.Count) - 1]);
                }
            }

            if (_moduleKnowledge.Any()) {
                currentCount += _moduleKnowledge.Count;
                if (randomValue <= currentCount) {
                    LoadingScreenHintsModule.Instance.SeenHints.Value.Add(randomValue);
                    return new ModuleKnowledgeHint(_moduleKnowledge[randomValue - (currentCount - _moduleKnowledge.Count) - 1]);
                }
            }

            if (_quotes.Any()) {
                currentCount += _quotes.Count;
                if (randomValue <= currentCount) {
                    LoadingScreenHintsModule.Instance.SeenHints.Value.Add(randomValue);
                    return new QuoteHint(_quotes[randomValue - (currentCount - _quotes.Count) - 1]);
                }
            }

            if (_characters.Any()) {
                currentCount += _characters.Count;
                if (randomValue <= currentCount) {
                    var characterHint = _characters[randomValue - (currentCount - _characters.Count) - 1];
                    characterHint.Texture?.Dispose();
                    characterHint.Texture = new AsyncTexture2D();
                    var imageBytes = await HttpUtil.TryAsync(() => $"{_baseUrl}characters/{characterHint.Image}".GetBytesAsync());
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

                    LoadingScreenHintsModule.Instance.SeenHints.Value.Add(randomValue);
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
                var modules = GameService.Module.Modules.Where(mm => mm.Enabled && 
                                                                     mm.Manifest.Namespace.StartsWith(moduleKnowledge.ManifestNamespace, StringComparison.InvariantCultureIgnoreCase)).ToList();
                var module = modules.FirstOrDefault();
                if (module != null) { // Filter hints for enabled modules.
                    moduleKnowledge.ModuleName = module.Manifest.Namespace.Equals(moduleKnowledge.ManifestNamespace, StringComparison.InvariantCultureIgnoreCase) ? module.Manifest.Name : string.Empty;
                    moduleKnowledge.Author     = module.Manifest.Author?.Username ?? module.Manifest.Contributors.FirstOrDefault()?.Username;
                    result.Add(moduleKnowledge);
                }
            }
            return result;
        }

        private async void OnUserLocaleChanged(object sender, ValueEventArgs<CultureInfo> e) {
            await LoadAsync(e.Value);
        }

        private void DisposeTextures() {
            if (_characters != null) {
                foreach (var character in _characters) {
                    character.Texture?.Dispose();
                }
            }
        }

        public void Dispose() {
            GameService.Overlay.UserLocaleChanged -= OnUserLocaleChanged;
            DisposeTextures();
        }
    }
}
