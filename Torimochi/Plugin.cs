using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Reflection;
using Torimochi.Installers;
using IPALogger = IPA.Logging.Logger;

namespace Torimochi
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        private Harmony _harmony;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Zenjector zenjector, Config conf)
        {
            Instance = this;
            Log = logger;
            Log.Info("Torimochi initialized.");
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
            zenjector.Install<TorimochiPlayerInstaller>(Location.Player);
            zenjector.Install<TorimochiMenuInstaller>(Location.Menu);
        }

        [OnEnable]
        public void OnEnable()
        {
            try {
                if (_harmony != null) {
                    _harmony.UnpatchSelf();
                    _harmony = null;
                }
                _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            }
            catch (System.Exception e) {
                Log.Error(e);
            }
        }
        [OnDisable]
        public void OnDisable()
        {
            try {

                if (_harmony != null) {
                    _harmony.UnpatchSelf();
                    _harmony = null;
                }
            }
            catch (System.Exception e) {
                Log.Error(e);
                throw;
            }
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
