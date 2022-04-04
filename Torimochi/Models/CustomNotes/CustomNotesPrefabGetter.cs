using SiraUtil.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torimochi.Configuration;
using UnityEngine;
using Zenject;

namespace Torimochi.Models.CustomNotes
{
    public class CustomNotesPrefabGetter : IDisposable
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プロパティ
        public ConcurrentDictionary<int, MemoryPoolContainer<SiraPrefabContainer>> NoteMeshes { get; private set; } = new ConcurrentDictionary<int, MemoryPoolContainer<SiraPrefabContainer>>();
        public ConcurrentDictionary<int, ConcurrentQueue<SiraPrefabContainer>> ActiveMeshes { get; private set; } = new ConcurrentDictionary<int, ConcurrentQueue<SiraPrefabContainer>>();
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private bool _disposedValue;
        private ConcurrentDictionary<int, GameObject> _meshes = new ConcurrentDictionary<int, GameObject>();
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
            if (!_disposedValue) {
                if (disposing) {
                    foreach (var protocol in Enum.GetValues(typeof(CustomNoteProtocol)).OfType<CustomNoteProtocol>()) {
                        var id = protocol.GetDescription();
                        if (id == protocol.ToString()) {
                            continue;
                        }
                        if (this.ActiveMeshes.TryGetValue((int)protocol, out var activeMeshes)) {
                            if (this.NoteMeshes.TryRemove((int)protocol, out var meshes)) {
                                while (activeMeshes.TryDequeue(out var activeMesh)) {
                                    meshes.Despawn(activeMesh);
                                }
                            }
                        }
                        while (this._meshes.TryRemove((int)protocol, out var mesh)) {
                            GameObject.Destroy(mesh.gameObject);
                        }
                    }
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
        #endregion
    }
}
