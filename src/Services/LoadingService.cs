using Blish_HUD;
using Nekres.Loading_Screen_Hints.Services.Controls.Hints;
using System;

namespace Nekres.Loading_Screen_Hints.Services {
    internal class LoadingService : IDisposable {

        private BaseHint _currentHint;

        public LoadingService() {
            GameService.GameIntegration.Gw2Instance.IsInGameChanged += OnGw2IsInGameChanged;
        }

        private void OnGw2IsInGameChanged(object sender, ValueEventArgs<bool> e) {
            if (e.Value) {

                // A hint is currently showing
                if (_currentHint is not {Fade: null}) {
                    return;
                }

                if (_currentHint.Opacity == 0.0f) {
                    _currentHint.Dispose();
                    _currentHint = null;
                } else {
                    _currentHint.FadeOut();
                }

            } else {

                if (_currentHint == null) {
                    _currentHint = LoadingScreenHintsModule.Instance.Resources.NextHint();
                }

            }
        }

        public void Dispose() {
            GameService.GameIntegration.Gw2Instance.IsInGameChanged -= OnGw2IsInGameChanged;
            _currentHint?.Dispose();
        }

    }
}
