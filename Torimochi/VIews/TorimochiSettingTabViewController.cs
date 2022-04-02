using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using Torimochi.Configuration;
using Zenject;

namespace Torimochi.VIews
{
    [HotReload]
    internal class TorimochiSettingTabViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [UIValue("enable")]
        /// <summary>有効かどうか を取得、設定</summary>
        public bool Enable
        {
            get => PluginConfig.Instance.Enable;

            set => PluginConfig.Instance.Enable = value;
        }

        private bool _disposedValue;

        public void Initialize()
        {
            GameplaySetup.instance?.RemoveTab("Torimochi");
            GameplaySetup.instance?.AddTab("Torimochi", string.Join(".", this.GetType().Namespace, this.GetType().Name), this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing) {
                    GameplaySetup.instance?.RemoveTab("Torimochi");
                }
                this._disposedValue = true;
            }
        }
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
