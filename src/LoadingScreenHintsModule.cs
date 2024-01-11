using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Nekres.Loading_Screen_Hints.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;

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
        internal SettingEntry<List<int>> SeenHints;

        internal ResourceService Resources;
        internal LoadingService  Loading;

        [ImportingConstructor]
        public LoadingScreenHintsModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) {
            Instance = this;
        }

        protected override void DefineSettings(SettingCollection settings) {
            var selfManagedSettings = settings.AddSubCollection("selfManaged", false, false);
            SeenHints = selfManagedSettings.DefineSetting("seen", new List<int>());
        }

        protected override void Initialize() {
            Resources = new ResourceService();
            Loading   = new LoadingService();
        }

        protected override async Task LoadAsync() {
            await Resources.LoadAsync(CultureInfo.CurrentUICulture);
        }

        protected override void OnModuleLoaded(EventArgs e) {
            base.OnModuleLoaded(e);
        }

        protected override void Unload() {
            Loading?.Dispose();
            Resources?.Dispose();
            Instance = null;
        }
    }
}
