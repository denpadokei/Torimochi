using SiraUtil.Objects;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Torimochi.Configuration;
using Zenject;

namespace Torimochi.Models.CustomNotes
{
    public class CustomNotesPrefabGetter : IDisposable
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プロパティ
        public ConcurrentDictionary<int, MemoryPoolContainer<SiraPrefabContainer>> NoteMeshes { get; } = new ConcurrentDictionary<int, MemoryPoolContainer<SiraPrefabContainer>>();
        public ConcurrentDictionary<int, ConcurrentQueue<SiraPrefabContainer>> ActiveMeshes { get; } = new ConcurrentDictionary<int, ConcurrentQueue<SiraPrefabContainer>>();
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private bool _disposedValue;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // 構築・破棄
        [Inject]
        public CustomNotesPrefabGetter(DiContainer container)
        {
            foreach (var protocol in Enum.GetValues(typeof(CustomNoteProtocol)).OfType<CustomNoteProtocol>()) {
                var id = protocol.GetDescription();
                if (id == protocol.ToString()) {
                    continue;
                }
                var pool = container.TryResolveId<SiraPrefabContainer.Pool>(id);
                if (pool == null) {
                    continue;
                }
                pool.ExpandBy(pool.NumTotal + PluginConfig.Instance.MaxNotesCount);
                var poolContaner = new MemoryPoolContainer<SiraPrefabContainer>(pool);
                this.NoteMeshes.TryAdd((int)protocol, poolContaner);
                this.ActiveMeshes.TryAdd((int)protocol, new ConcurrentQueue<SiraPrefabContainer>());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing) {
                    foreach (var protocol in Enum.GetValues(typeof(CustomNoteProtocol)).OfType<CustomNoteProtocol>()) {
                        var id = protocol.GetDescription();
                        if (id == protocol.ToString()) {
                            continue;
                        }
                        if (this.ActiveMeshes.TryRemove((int)protocol, out var activeMeshes)) {
                            if (this.NoteMeshes.TryRemove((int)protocol, out var meshes)) {
                                while (activeMeshes.TryDequeue(out var activeMesh)) {
                                    if (activeMesh == null) {
                                        continue;
                                    }
                                    meshes.Despawn(activeMesh);
                                }
                            }
                        }
                    }
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
        #endregion
    }
}
