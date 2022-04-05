using IPA.Utilities;
using System;
using System.Collections.Concurrent;
using System.Linq;
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
            this.SetUpPools();
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プライベートメソッド
        private void OnNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK) {
                return;
            }
            try {
                this.ActiveMesh(in noteCutInfo);
            }
            catch (Exception e) {
                Plugin.Log.Error(e);
            }
        }

        private MeshRenderer GetMeshRenderer(GameObject controller)
        {
            var disappearingArrowController = controller.GetComponentsInChildren<DisappearingArrowController>().FirstOrDefault();
            var noteMesh = disappearingArrowController.GetField<MeshRenderer, DisappearingArrowControllerBase<GameNoteController>>("_cubeMeshRenderer");
            noteMesh.transform.SetParent(null, false);
            return noteMesh;
        }

        private void SetUpPools()
        {
            this._noteMeshPool = new ObjectMemoryPool<MeshRenderer>(this._maxNoteCount,
                () =>
                {
                    var go = new GameObject("CloneNoteMesh");
                    var meshClone = GameObject.Instantiate(this._noteMeshRendrer);
                    meshClone.transform.SetParent(go.transform);
                    meshClone.transform.localPosition = Vector3.zero;
                    meshClone.transform.localRotation = Quaternion.identity;
                    meshClone.gameObject.SetActive(true);
                    foreach (var item in go.GetComponentsInChildren<BoxCollider>(true)) {
                        GameObject.Destroy(item.gameObject);
                    }
                    foreach (var target in go.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                    return meshClone;
                },
                null,
                mesh =>
                {
                    if (PluginConfig.Instance.ShowHMDCam) {
                        this.SetGameObjectLayer(mesh.gameObject, 0);
                    }
                    else {
                        this.SetGameObjectLayer(mesh.gameObject, s_thirdPersonOnlyLayer);
                    }
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>(true)) {
                        target.enabled = true;
                        target.forceRenderingOff = false;
                    }
                    this._activeMesh.Enqueue(mesh);
                },
                mesh =>
                {
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                });
            this._noteHeadMeshPool = new ObjectMemoryPool<MeshRenderer>(this._maxNoteCount,
                () =>
                {
                    var go = new GameObject("CloneNoteHeadMesh");
                    var meshClone = GameObject.Instantiate(this._noteHeadRendrer);
                    meshClone.transform.SetParent(go.transform);
                    meshClone.transform.localPosition = Vector3.zero;
                    meshClone.transform.localRotation = Quaternion.identity;
                    foreach (var item in go.GetComponentsInChildren<BoxCollider>(true)) {
                        GameObject.Destroy(item.gameObject);
                    }
                    foreach (var target in go.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                    return meshClone;
                },
                null,
                mesh =>
                {
                    if (PluginConfig.Instance.ShowHMDCam) {
                        this.SetGameObjectLayer(mesh.gameObject, 0);
                    }
                    else {
                        this.SetGameObjectLayer(mesh.gameObject, s_thirdPersonOnlyLayer);
                    }
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>(true)) {
                        target.enabled = true;
                        target.forceRenderingOff = false;
                    }
                    this._activeHeadMesh.Enqueue(mesh);
                },
                mesh =>
                {
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                });
            if (this._noteSliderRendrer != null) {
                this._noteSliderMeshPool = new ObjectMemoryPool<MeshRenderer>(this._maxNoteCount,
                () =>
                {
                    var go = new GameObject("CloneNoteSliderMesh");
                    var meshClone = GameObject.Instantiate(this._noteSliderRendrer);
                    meshClone.transform.SetParent(go.transform);
                    meshClone.transform.localPosition = Vector3.zero;
                    meshClone.transform.localRotation = Quaternion.identity;
                    foreach (var item in go.GetComponentsInChildren<BoxCollider>(true)) {
                        GameObject.Destroy(item.gameObject);
                    }
                    foreach (var target in go.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                    return meshClone;
                },
                null,
                mesh =>
                {
                    if (PluginConfig.Instance.ShowHMDCam) {
                        this.SetGameObjectLayer(mesh.gameObject, 0);
                    }
                    else {
                        this.SetGameObjectLayer(mesh.gameObject, s_thirdPersonOnlyLayer);
                    }
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>(true)) {
                        target.enabled = true;
                        target.forceRenderingOff = false;
                    }
                    this._activeSliderMesh.Enqueue(mesh);
                },
                mesh =>
                {
                    foreach (var target in mesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        target.enabled = false;
                        target.forceRenderingOff = true;
                    }
                });
            }
        }

        private void ActiveMesh(in NoteCutInfo noteCutInfo)
        {
            var color = this._colorManager.ColorForType(noteCutInfo.noteData.colorType);
            switch (noteCutInfo.noteData.gameplayType) {
                case NoteData.GameplayType.Normal:
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
                    var arrowGlow = noteMesh.gameObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == s_arrowGlowName);
                    var circleGlow = noteMesh.gameObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == s_circleGlowName);
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
                    while (this._maxNoteCount - 1 < this._noteMeshPool.ActiveComponentCount && this._activeMesh.TryDequeue(out var oldActiveNote)) {
                        oldActiveNote.gameObject.transform.SetParent(null);
                        this._noteMeshPool.Free(oldActiveNote);
                    }
                    break;
                case NoteData.GameplayType.BurstSliderHead:
                    var noteHeadMesh = this._noteHeadMeshPool.Alloc();
                    switch (noteCutInfo.saberType) {
                        case SaberType.SaberA:
                            var leftSaber = this._saberManager.leftSaber;
                            noteHeadMesh.gameObject.transform.SetParent(leftSaber.transform, false);
                            break;
                        case SaberType.SaberB:
                            var rightSaber = this._saberManager.rightSaber;
                            noteHeadMesh.gameObject.transform.SetParent(rightSaber.transform, false);
                            break;
                        default:
                            break;
                    }
                    foreach (var item in noteHeadMesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        item.forceRenderingOff = false;
                    }
                    foreach (var target in noteHeadMesh.gameObject.GetComponentsInChildren<MaterialPropertyBlockController>()) {
                        var prop = target.materialPropertyBlock;
                        if (target.name == s_arrowName) {
                            prop.SetColor(s_colorId, Color.white.ColorWithAlpha(1f));
                        }
                        else {
                            prop.SetColor(s_colorId, color.ColorWithAlpha(1f));
                        }
                        target.ApplyChanges();
                    }
                    noteHeadMesh.transform.position = noteCutInfo.notePosition;
                    noteHeadMesh.transform.rotation = noteCutInfo.noteRotation;
                    while (this._maxNoteCount - 1 < this._noteHeadMeshPool.ActiveComponentCount && this._activeHeadMesh.TryDequeue(out var oldActiveNote)) {
                        oldActiveNote.gameObject.transform.SetParent(null);
                        this._noteHeadMeshPool.Free(oldActiveNote);
                    }
                    break;
                case NoteData.GameplayType.BurstSliderElement:
                    var noteSliderMesh = this._noteSliderMeshPool.Alloc();
                    switch (noteCutInfo.saberType) {
                        case SaberType.SaberA:
                            var leftSaber = this._saberManager.leftSaber;
                            noteSliderMesh.gameObject.transform.SetParent(leftSaber.transform, false);
                            break;
                        case SaberType.SaberB:
                            var rightSaber = this._saberManager.rightSaber;
                            noteSliderMesh.gameObject.transform.SetParent(rightSaber.transform, false);
                            break;
                        default:
                            break;
                    }
                    foreach (var item in noteSliderMesh.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        item.forceRenderingOff = false;
                    }
                    foreach (var target in noteSliderMesh.gameObject.GetComponentsInChildren<MaterialPropertyBlockController>()) {
                        var prop = target.materialPropertyBlock;
                        prop.SetColor(s_colorId, color.ColorWithAlpha(1f));
                        target.ApplyChanges();
                    }
                    noteSliderMesh.transform.position = noteCutInfo.notePosition;
                    noteSliderMesh.transform.rotation = noteCutInfo.noteRotation;
                    while (this._maxNoteCount - 1 < this._noteSliderMeshPool.ActiveComponentCount && this._activeSliderMesh.TryDequeue(out var oldActiveNote)) {
                        oldActiveNote.gameObject.transform.SetParent(null);
                        this._noteSliderMeshPool.Free(oldActiveNote);
                    }
                    break;
                case NoteData.GameplayType.BurstSliderElementFill:
                case NoteData.GameplayType.Bomb:
                default:
                    break;
            }
        }

        private void SetGameObjectLayer(GameObject go, int layer)
        {
            if (go == null) {
                return;
            }
            if (go.layer == layer) {
                return;
            }
            go.layer = layer;
            foreach (var transform in go.GetComponentsInChildren<Transform>(true)) {
                transform.gameObject.layer = layer;
            }
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private readonly SaberManager _saberManager;
        private readonly BeatmapObjectManager _beatmapObjectManager;
        protected ColorManager _colorManager;
        private ObjectMemoryPool<MeshRenderer> _noteMeshPool;
        private ObjectMemoryPool<MeshRenderer> _noteHeadMeshPool;
        private ObjectMemoryPool<MeshRenderer> _noteSliderMeshPool;
        private readonly ConcurrentQueue<MeshRenderer> _activeMesh = new ConcurrentQueue<MeshRenderer>();
        private readonly ConcurrentQueue<MeshRenderer> _activeHeadMesh = new ConcurrentQueue<MeshRenderer>();
        private readonly ConcurrentQueue<MeshRenderer> _activeSliderMesh = new ConcurrentQueue<MeshRenderer>();
        private readonly MeshRenderer _noteMeshRendrer;
        private readonly MeshRenderer _noteHeadRendrer;
        private readonly MeshRenderer _noteSliderRendrer;
        private bool _disposedValue;
        private static readonly int s_colorId = Shader.PropertyToID("_Color");
        private static readonly string s_arrowName = "NoteArrow";
        private static readonly string s_arrowGlowName = "NoteArrowGlow";
        private static readonly string s_circleGlowName = "NoteCircleGlow";
        private readonly uint _maxNoteCount;
        private static readonly int s_thirdPersonOnlyLayer = 3;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // 構築・破棄
        [Inject]
        public TorimochiController(
            SaberManager saberManager,
            BeatmapObjectManager beatmapObjectManager,
            ColorManager colorManager,
            [Inject(Id = NoteData.GameplayType.Normal)] GameNoteController.Pool basicGameNotePool,
            [Inject(Id = NoteData.GameplayType.BurstSliderHead)] GameNoteController.Pool headGameNotePool,
            [InjectOptional(Id = NoteData.GameplayType.BurstSliderElement)] GameNoteController.Pool sliderNotePool)
        {
            this._saberManager = saberManager;
            this._beatmapObjectManager = beatmapObjectManager;
            this._colorManager = colorManager;
            // 絶対なんかよろしくない気がする。
            var note = basicGameNotePool.Spawn();
            var clone = GameObject.Instantiate(note.gameObject);
            this._noteMeshRendrer = this.GetMeshRenderer(clone);
            this._noteMeshRendrer.enabled = false;
            this._noteMeshRendrer.gameObject.SetActive(false);
            GameObject.Destroy(clone);
            basicGameNotePool.Despawn(note);
            note = headGameNotePool.Spawn();
            clone = GameObject.Instantiate(note.gameObject);
            this._noteHeadRendrer = this.GetMeshRenderer(clone);
            this._noteHeadRendrer.enabled = false;
            this._noteHeadRendrer.gameObject.SetActive(false);
            GameObject.Destroy(clone);
            headGameNotePool.Despawn(note);
            if (sliderNotePool != null) {
                note = sliderNotePool.Spawn();
                clone = GameObject.Instantiate(note.gameObject);
                this._noteSliderRendrer = this.GetMeshRenderer(clone);
                this._noteSliderRendrer.enabled = false;
                this._noteSliderRendrer.gameObject.SetActive(false);
                GameObject.Destroy(clone);
                sliderNotePool.Despawn(note);
            }

            this._maxNoteCount = (uint)PluginConfig.Instance.MaxNotesCount;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing) {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    GameObject.Destroy(this._noteMeshRendrer);
                    while (this._activeMesh.TryDequeue(out var mesh)) {
                        this._noteMeshPool.Free(mesh);
                    }
                    this._noteMeshPool.Dispose();
                    while (this._activeHeadMesh.TryDequeue(out var mesh)) {
                        this._noteHeadMeshPool.Free(mesh);
                    }
                    this._noteHeadMeshPool.Dispose();
                    while (this._activeSliderMesh.TryDequeue(out var mesh)) {
                        this._noteSliderMeshPool?.Free(mesh);
                    }
                    this._noteSliderMeshPool?.Dispose();
                    this._beatmapObjectManager.noteWasCutEvent -= this.OnNoteWasCutEvent;
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
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
    }
}
