using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            GameplaySetup.instance?.AddTab("Torimochi", String.Join(".", this.GetType().Namespace, this.GetType().Name), this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue) {
                if (disposing) {
                    GameplaySetup.instance?.RemoveTab("Torimochi");
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
