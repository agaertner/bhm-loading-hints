using Blish_HUD;
using Nekres.Loading_Screen_Hints.Services.Controls.Hints;
using System;

namespace Nekres.Loading_Screen_Hints.Services {
    internal class LoadingService : IDisposable {

        private BaseHint _currentHint;

        public LoadingService() {
            GameService.GameIntegration.Gw2Instance.IsInGameChanged += OnGw2IsInGameChanged;
        }

        private async void OnGw2IsInGameChanged(object sender, ValueEventArgs<bool> e) {
            if (e.Value) {

                _currentHint?.FadeOut();

            } else {

                if (string.IsNullOrWhiteSpace(GameService.Gw2Mumble.PlayerCharacter.Name)) {
                    return; // Never went past character selection.
                }

                _currentHint?.Dispose();
                _currentHint = await LoadingScreenHintsModule.Instance.Resources.NextHint();

            }
        }

        public void Dispose() {
            GameService.GameIntegration.Gw2Instance.IsInGameChanged -= OnGw2IsInGameChanged;
            _currentHint?.Dispose();
        }

    }
}
