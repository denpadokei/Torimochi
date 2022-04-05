using IPA.Loader;
using System;
using Torimochi.Configuration;
using Torimochi.Models.CustomNotes;
using Zenject;

namespace Torimochi.Installers
{
    public class TorimochiPlayerInstaller : Installer
    {
        private object _loader;
        public int SelectedNoteIndex => this._loader == null ? -1 : (int)this._loader.GetType().GetProperty("SelectedNote").GetValue(this._loader);
        public bool Enabled => this._loader != null && (bool)this._loader.GetType().GetProperty("Enabled").GetValue(this._loader);
        public override void InstallBindings()
        {
            if (!PluginConfig.Instance.Enable) {
                return;
            }
            var loaderType = Type.GetType("CustomNotes.Managers.NoteAssetLoader, CustomNotes");
            if (loaderType == null) {
                this._loader = null;
            }
            else {
                this._loader = this.Container.TryResolve(loaderType);
            }
            if (PluginManager.GetPluginFromId("Custom Notes") != null && this.Enabled && 1 <= this.SelectedNoteIndex) {
                this.Container.BindInterfacesAndSelfTo<CustomNotesPrefabGetter>().AsCached();
                this.Container.BindInterfacesAndSelfTo<TorimochiCustomNotesController>().AsCached().NonLazy();
            }
            else {
                this.Container.BindInterfacesAndSelfTo<TorimochiController>().AsCached().NonLazy();
            }
        }
    }
}
