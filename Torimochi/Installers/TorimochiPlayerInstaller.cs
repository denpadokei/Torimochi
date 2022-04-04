using IPA.Loader;
using System;
using Torimochi.Configuration;
using Torimochi.Models.CustomNotes;
using Zenject;

namespace Torimochi.Installers
{
    public class TorimochiPlayerInstaller : Installer
    {
        private static object _loader;
        public static int SelectedNoteIndex => _loader == null ? -1 : (int)_loader.GetType().GetProperty("SelectedNote").GetValue(_loader);
        public override void InstallBindings()
        {
            if (!PluginConfig.Instance.Enable) {
                return;
            }
            var loaderType = Type.GetType("CustomNotes.Managers.NoteAssetLoader, CustomNotes");
            if (loaderType == null) {
                _loader = null;
            }
            else {
                _loader = this.Container.TryResolve(loaderType);
            }
            if (PluginManager.GetPluginFromId("Custom Notes") != null && 1 <= SelectedNoteIndex) {
                this.Container.BindInterfacesAndSelfTo<CustomNotesPrefabGetter>().AsCached();
                this.Container.BindInterfacesAndSelfTo<TorimochiCustomNotesController>().AsCached().NonLazy();
            }
            else {
                this.Container.BindInterfacesAndSelfTo<TorimochiController>().AsCached().NonLazy();
            }
        }
    }
}
