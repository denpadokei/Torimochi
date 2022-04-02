using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torimochi.Configuration;
using Torimochi.Models;
using UnityEngine;
using Zenject;

namespace Torimochi
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class TorimochiController : IInitializable, IDisposable
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プロパティ
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // コマンド
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // コマンド用メソッド
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // オーバーライドメソッド
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // パブリックメソッド
        public void Initialize()
        {
            this._beatmapObjectManager.noteWasCutEvent += this.OnNoteWasCutEvent;
            var noteController = Resources.FindObjectsOfTypeAll<GameNoteController>().FirstOrDefault();
            this._noteMeshPool = new ObjectMemoryPool<MeshRenderer>(this._maxNoteCount,
                () =>
                {
                    var go = new GameObject("CloneNoteMesh");
                    var meshClone = GameObject.Instantiate(this._noteMeshRendrer);
                    meshClone.transform.SetParent(go.transform);
                    meshClone.transform.localPosition = Vector3.zero;
                    meshClone.transform.localRotation = Quaternion.identity;
                    meshClone.enabled = true;
                    foreach (var item in go.GetComponentsInChildren<BoxCollider>(true)) {
                        GameObject.Destroy(item);
                    }
                    return meshClone;
                },
                null,
                mesh =>
                {
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = true;
                        target.forceRenderingOff = false;
                    }
                    this._activeMesh.Enqueue(mesh);
                    this._activeNoteCount++;
                },
                mesh =>
                {
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                    this._activeNoteCount--;
                });
        }

        private void OnNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK) {
                return;
            }
            try {
                var noteMesh = this._noteMeshPool.Alloc();
                switch (noteCutInfo.saberType) {
                    case SaberType.SaberA:
                        var leftSaber = this._saberManager.leftSaber;
                        noteMesh.gameObject.transform.SetParent(leftSaber.transform, false);
                        break;
                    case SaberType.SaberB:
                        var rightSaber = this._saberManager.rightSaber;
                        noteMesh.gameObject.transform.SetParent(rightSaber.transform, false);
                        break;
                    default:
                        break;
                }
                var color = this._colorManager.ColorForType(noteController.noteData.colorType);
                var isAny = false;
                switch (noteCutInfo.noteData.cutDirection) {
                    case NoteCutDirection.Any:
                        isAny = true;
                        break;
                    case NoteCutDirection.Up:
                    case NoteCutDirection.Down:
                    case NoteCutDirection.Left:
                    case NoteCutDirection.Right:
                    case NoteCutDirection.UpLeft:
                    case NoteCutDirection.UpRight:
                    case NoteCutDirection.DownLeft:
                    case NoteCutDirection.DownRight:
                    case NoteCutDirection.None:
                    default:
                        isAny = false;
                        break;
                }
                var arrow = noteMesh.gameObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == s_arrowName);
                var arrowGlow = noteMesh.gameObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == "NoteArrowGlow");
                var circleGlow = noteMesh.gameObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == "NoteCircleGlow");
                arrow.forceRenderingOff = isAny;
                arrowGlow.forceRenderingOff = isAny;
                circleGlow.forceRenderingOff = !isAny;
                foreach (var target in noteMesh.gameObject.GetComponentsInChildren<MaterialPropertyBlockController>()) {
                    var prop = target.materialPropertyBlock;
                    if (target.name == s_arrowName) {
                        prop.SetColor(s_colorId, Color.white.ColorWithAlpha(1f));
                    }
                    else {
                        prop.SetColor(s_colorId, color.ColorWithAlpha(1f));
                    }
                    target.ApplyChanges();
                }
                noteMesh.transform.position = noteCutInfo.notePosition;
                noteMesh.transform.rotation = noteCutInfo.noteRotation;
                while (this._maxNoteCount - 1 < this._activeNoteCount && this._activeMesh.TryDequeue(out var oldActiveNote)) {
                    oldActiveNote.gameObject.transform.SetParent(null);
                    this._noteMeshPool.Free(oldActiveNote);
                }
            }
            catch (Exception e) {
                Plugin.Log.Error(e);
            }
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プライベートメソッド
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private SaberManager _saberManager;
        private BeatmapObjectManager _beatmapObjectManager;
        protected ColorManager _colorManager;
        private ObjectMemoryPool<MeshRenderer> _noteMeshPool;
        private ConcurrentQueue<MeshRenderer> _activeMesh = new ConcurrentQueue<MeshRenderer>();
        private MeshRenderer _noteMeshRendrer;
        private bool _disposedValue;
        int _activeNoteCount = 0;
        private static readonly int s_colorId = Shader.PropertyToID("_Color");
        private static readonly string s_arrowName = "NoteArrow";
        private int _maxNoteCount;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // 構築・破棄
        [Inject]
        public TorimochiController(SaberManager saberManager, BeatmapObjectManager beatmapObjectManager, ColorManager colorManager, [Inject(Id = NoteData.GameplayType.Normal)] GameNoteController.Pool basicGameNotePool)
        {
            this._saberManager = saberManager;
            this._beatmapObjectManager = beatmapObjectManager;
            this._colorManager = colorManager;
            // 絶対なんかよろしくない気がする。
            var note = basicGameNotePool.Spawn();
            var disappearingArrowController = note.gameObject.GetComponentInParent<DisappearingArrowController>();
            var noteMesh = disappearingArrowController.GetField<MeshRenderer, DisappearingArrowControllerBase<GameNoteController>>("_cubeMeshRenderer");
            basicGameNotePool.Despawn(note);
            this._noteMeshRendrer = GameObject.Instantiate(noteMesh);
            this._noteMeshRendrer.transform.SetParent(null);
            this._noteMeshRendrer.enabled = false;
            this._maxNoteCount = PluginConfig.Instance.MaxNotesCount;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue) {
                if (disposing) {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    GameObject.Destroy(this._noteMeshRendrer);
                    this._noteMeshPool.Dispose();
                    this._beatmapObjectManager.noteWasCutEvent -= this.OnNoteWasCutEvent;
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
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
    }
}
