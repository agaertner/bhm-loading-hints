using Blish_HUD;
using Microsoft.Xna.Framework;
using Nekres.Loading_Screen_Hints.Services.Controls.Hints;
using System;

namespace Nekres.Loading_Screen_Hints.Services {
    internal class LoadingService : IDisposable {

        private BaseHint _currentHint;

        private Vector3? _postLoadPosition;

        public LoadingService() {
            GameService.GameIntegration.Gw2Instance.IsInGameChanged += OnGw2IsInGameChanged;
        }

        private async void OnGw2IsInGameChanged(object sender, ValueEventArgs<bool> e) {
            if (e.Value) {

                _postLoadPosition = GameService.Gw2Mumble.PlayerCharacter.Position;

                if (!LoadingScreenHintsModule.Instance.HideOnMovement.Value) {
                    _currentHint?.FadeOut();
                }

            } else {

                _postLoadPosition = null;

                if (string.IsNullOrWhiteSpace(GameService.Gw2Mumble.PlayerCharacter.Name)) {
                    return; // Never went past character selection.
                }

                _currentHint?.Dispose();
                _currentHint = await LoadingScreenHintsModule.Instance.Resources.NextHint();

            }
        }

        public void Update(GameTime gameTime) {
            if (!GameService.GameIntegration.Gw2Instance.IsInGame) {
                return;
            }

            if (LoadingScreenHintsModule.Instance.HideOnMovement.Value && _postLoadPosition != null) {
                if ((_postLoadPosition.Value - GameService.Gw2Mumble.PlayerCharacter.Position).Length() > 0.3) {
                    _currentHint?.FadeOut(); // Character has moved.
                    _postLoadPosition = null;
                }
            }
        }

        public void Dispose() {
            GameService.GameIntegration.Gw2Instance.IsInGameChanged -= OnGw2IsInGameChanged;
            _currentHint?.Dispose();
        }
    }
}
