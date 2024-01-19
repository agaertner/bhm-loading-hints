using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Nekres.Loading_Screen_Hints.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static Nekres.Loading_Screen_Hints.Services.ResourceService;

namespace Nekres.Loading_Screen_Hints {

    [Export(typeof(Module))]
    public class LoadingScreenHintsModule : Module {

        internal static readonly Logger Logger = Logger.GetLogger(typeof(LoadingScreenHintsModule));

        internal static LoadingScreenHintsModule Instance;

        #region Service Managers
        internal SettingsManager    SettingsManager    => this.ModuleParameters.SettingsManager;
        internal ContentsManager    ContentsManager    => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager      Gw2ApiManager      => this.ModuleParameters.Gw2ApiManager;
        #endregion

        // Settings
        internal SettingEntry<List<int>>        SeenHints;
        internal SettingEntry<SupportedLocales> LanguageOverride;
        internal SettingEntry<bool>             EnableCharacterRiddles;
        internal SettingEntry<bool>             EnableQuotations;
        internal SettingEntry<bool>             EnableHints;
        internal SettingEntry<bool>             HideOnMovement;

        internal ResourceService                Resources;
        internal LoadingService                 Loading;

        private double _lastRun;

        [ImportingConstructor]
        public LoadingScreenHintsModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) {
            Instance = this;
        }

        protected override void DefineSettings(SettingCollection settings) {
            var selfManagedSettings = settings.AddSubCollection("selfManaged", false, false);
            SeenHints = selfManagedSettings.DefineSetting("seen", new List<int>());

            var options = settings.AddSubCollection("options", true, () => Properties.Resources.Options);
            EnableCharacterRiddles = options.DefineSetting("characterRiddles", true, 
                                                           () => $"{Properties.Resources.Enable} {Properties.Resources.Character_Riddles}");
            EnableQuotations       = options.DefineSetting("quotations",       true, 
                                                           () => $"{Properties.Resources.Enable} {Properties.Resources.Quotations}");
            EnableHints            = options.DefineSetting("hints",            true, 
                                                           () => $"{Properties.Resources.Enable} {Properties.Resources.Hints}");
            HideOnMovement = options.DefineSetting("hideOnMovement", true, 
                                                   () => Properties.Resources.Hide_on_Movement);

            LanguageOverride = settings.DefineSetting("languageOverride", SupportedLocales.None,
                                                      () => Properties.Resources.Language_Override,
                                                      () => Properties.Resources.Forces_a_different_language_for_displaying_hints_);
        }

        protected override void Initialize() {
            Resources = new ResourceService();
            Loading   = new LoadingService();
        }

        protected override void Update(GameTime gameTime) {
            _lastRun = gameTime.ElapsedGameTime.TotalMilliseconds;

            // Rate limit update
            if (gameTime.TotalGameTime.TotalMilliseconds - _lastRun < 10) {
                return;
            }

            Loading.Update(gameTime);
        }

        protected override async Task LoadAsync() {
            await Resources.LoadAsync();
        }

        protected override void Unload() {
            Loading?.Dispose();
            Resources?.Dispose();
            Instance = null;
        }
    }
}
