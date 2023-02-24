using HarmonyLib;
using UnityEngine;

namespace Torimochi.Patches
{
    [HarmonyPatch(typeof(NoteBigCuttableColliderSize), nameof(NoteBigCuttableColliderSize.Awake))]
    public class NoteBigCuttableColliderSizePatch
    {

        [HarmonyPrefix]
        public static bool AwakePatch(NoteBigCuttableColliderSize __instance, ref NoteController ____noteController, ref Vector3 ____defaultColliderSize, ref BoxCollider ____boxCollider)
        {
            if (____boxCollider != null) {
                ____boxCollider.size = ____defaultColliderSize;
            }
            if (____noteController != null) {
                ____noteController.didInitEvent.Add(__instance);
            }
            return false;
        }
    }
}
