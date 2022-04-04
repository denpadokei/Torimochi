﻿using System;
using Torimochi.Configuration;
using Torimochi.Models.CustomNotes;
using UnityEngine;
using Zenject;

namespace Torimochi
{
    public class TorimochiCustomNotesController : IInitializable, IDisposable
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // パブリックメソッド
        public void Initialize()
        {
            this._beatmapObjectManager.noteWasCutEvent += this.OnNoteWasCutEvent;
        }

        private void OnNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK) {
                return;
            }
            this.ActiveMesh(noteCutInfo);
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プライベートメソッド
        private void ActiveMesh(in NoteCutInfo noteCutInfo)
        {
            var cuttingType = CustomNoteProtocol.None;
            switch (noteCutInfo.saberType) {
                case SaberType.SaberA:
                    cuttingType |= CustomNoteProtocol.Left;
                    break;
                case SaberType.SaberB:
                    cuttingType |= CustomNoteProtocol.Right;
                    break;
                default:
                    break;
            }
            switch (noteCutInfo.noteData.cutDirection) {
                case NoteCutDirection.Up:
                case NoteCutDirection.Down:
                case NoteCutDirection.Left:
                case NoteCutDirection.Right:
                case NoteCutDirection.UpLeft:
                case NoteCutDirection.UpRight:
                case NoteCutDirection.DownLeft:
                case NoteCutDirection.DownRight:
                    cuttingType |= CustomNoteProtocol.Arrow;
                    break;
                case NoteCutDirection.Any:
                case NoteCutDirection.None:
                    cuttingType |= CustomNoteProtocol.Any;
                    break;
                default:
                    break;
            }
            switch (noteCutInfo.noteData.gameplayType) {
                case NoteData.GameplayType.Normal:
                    cuttingType |= CustomNoteProtocol.Normal;
                    break;
                case NoteData.GameplayType.Bomb:
                    cuttingType |= CustomNoteProtocol.Bomb;
                    break;
                case NoteData.GameplayType.BurstSliderHead:
                    cuttingType = CustomNoteProtocol.BurstSliderHead;
                    break;
                case NoteData.GameplayType.BurstSliderElement:
                    cuttingType |= CustomNoteProtocol.BurstSliderElement;
                    break;
                case NoteData.GameplayType.BurstSliderElementFill:
                    cuttingType |= CustomNoteProtocol.BurstSliderElementFill;
                    break;
                default:
                    break;
            }
            if (this._customNotesPrefabGetter.NoteMeshes.TryGetValue((int)cuttingType, out var meshes)) {
                var color = this._colorManager.ColorForType(noteCutInfo.noteData.colorType);
                this._customNotesPrefabGetter.ActiveMeshes.TryGetValue((int)cuttingType, out var activeMeshes);
                var container = meshes.Spawn();
                activeMeshes.Enqueue(container);
                var mesh = container;
                switch (noteCutInfo.saberType) {
                    case SaberType.SaberA:
                        var leftSaber = this._saberManager.leftSaber;
                        mesh.transform.SetParent(leftSaber.transform, false);
                        break;
                    case SaberType.SaberB:
                        var rightSaber = this._saberManager.rightSaber;
                        mesh.transform.SetParent(rightSaber.transform, false);
                        break;
                    default:
                        break;
                }
                foreach (var target in mesh.gameObject.GetComponentsInChildren<MaterialPropertyBlockController>()) {
                    var prop = target.materialPropertyBlock;
                    prop.SetColor(s_colorId, color.ColorWithAlpha(1f));
                    target.ApplyChanges();
                }
                mesh.transform.position = noteCutInfo.notePosition;
                mesh.transform.rotation = noteCutInfo.noteRotation;
                mesh.transform.localScale = Vector3.one;
                mesh.Prefab.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                mesh.Prefab.SetActive(true);
                while (this._maxNoteCount - 1 < meshes.activeItems.Count && activeMeshes.TryDequeue(out var oldActiveNote)) {
                    oldActiveNote.transform.SetParent(null);
                    oldActiveNote.Prefab.SetActive(false);
                    meshes.Despawn(oldActiveNote);
                }
            }
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private readonly SaberManager _saberManager;
        private readonly BeatmapObjectManager _beatmapObjectManager;
        private readonly ColorManager _colorManager;
        private readonly CustomNotesPrefabGetter _customNotesPrefabGetter;
        private bool _disposedValue;
        private static readonly int s_colorId = Shader.PropertyToID("_Color");
        private readonly uint _maxNoteCount;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // 構築・破棄
        [Inject]
        public TorimochiCustomNotesController(
            SaberManager saberManager,
            BeatmapObjectManager beatmapObjectManager,
            ColorManager colorManager,
            CustomNotesPrefabGetter customNotesPrefabGetter)
        {
            this._saberManager = saberManager;
            this._beatmapObjectManager = beatmapObjectManager;
            this._colorManager = colorManager;
            this._customNotesPrefabGetter = customNotesPrefabGetter;
            this._maxNoteCount = (uint)PluginConfig.Instance.MaxNotesCount;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing) {
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
    }
}
