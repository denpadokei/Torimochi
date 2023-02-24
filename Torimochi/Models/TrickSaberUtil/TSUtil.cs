using IPA.Loader;
using System;
using System.IO;
using System.Reflection;
using Zenject;

namespace Torimochi.Model.TrickSaberUtil
{
    public class TSUtil
    {
        public static bool TryGetSaberTrickModel(DiContainer container, SaberType saber, out Type saberTrickModel, out object instance)
        {
            saberTrickModel = null;
            instance = null;
            var tsPlugin = PluginManager.GetPlugin("TrickSaber");
            if (tsPlugin == null) {
                return false;
            }
            if (tsPlugin.HVersion < new Hive.Versioning.Version("1.1.3") || new Hive.Versioning.Version("2.0.0") <= tsPlugin.HVersion) {
                return false;
            }
            var tsDllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TrickSaber.dll");
            try {
                var tsAssembly = Assembly.LoadFrom(tsDllPath);
                var saberTrickMng = tsAssembly.GetType("TrickSaber.SaberTrickManager");
                var mng = container.TryResolveId(saberTrickMng, saber);
                var sbModelField = saberTrickMng.GetField("SaberTrickModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                instance = sbModelField.GetValue(mng);
                saberTrickModel = instance.GetType();
            }
            catch (Exception e) {
                Plugin.Log.Error(e);
                return false;
            }
            return saberTrickModel != null && instance != null;
        }
    }
}
