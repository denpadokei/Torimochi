using IPA.Loader;
using System;
using UnityEngine;
using Zenject;

namespace Torimochi.Models.CustomNotes
{
    public class CustomNoteUtil
    {
        public static bool IsInstallCustomNote { get; private set; }
        private static object _loader;
        public static int SelectedNoteIndex => _loader == null ? -1 : (int)_loader.GetType().GetProperty("SelectedNote").GetValue(_loader);

        [Inject]
        public void Init(DiContainer container)
        {
            var loaderType = Type.GetType("CustomNotes.Managers.NoteAssetLoader, CustomNotes");
            if (loaderType == null) {
                _loader = null;
            }
            else {
                _loader = container.TryResolve(loaderType);
            }
        }

        static CustomNoteUtil()
        {
            IsInstallCustomNote = PluginManager.GetPluginFromId("Custom Notes") != null;
        }

        public static bool TryGetColorNoteVisuals(GameObject gameObject, out ColorNoteVisuals colorNoteVisuals)
        {
            colorNoteVisuals = gameObject.GetComponentInChildren<ColorNoteVisuals>();
            if (colorNoteVisuals == null) {
                var customColorType = Type.GetType("CustomNotes.Overrides.CustomNoteColorNoteVisuals, CustomNotes");
                colorNoteVisuals = (ColorNoteVisuals)gameObject.GetComponentInChildren(customColorType);
            }
            return colorNoteVisuals != null;
        }

        public static bool TryGetGameNoteController(GameObject gameObject, out GameNoteController component)
        {
            component = gameObject.GetComponentInChildren<GameNoteController>();
            return component != null;
        }
    }
}