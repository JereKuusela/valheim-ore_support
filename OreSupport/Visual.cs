using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace OreSupport {
  public class Visual {

    private static Collider[] tempColliders = new Collider[128];
    public static void DrawSupport(MineRock5 obj) {
      Drawer.Remove(obj, Constants.SupportTag);
      if (Settings.LineWidth == 0) return;
      var areas = Patch.HitAreas(obj);
      if (areas.Count() < Settings.MinSize) return;
      var remaining = areas.Count(area => Patch.Health(area) > 0f);
      var index = -1;
      var supportedCount = 0;
      foreach (var area in areas) {
        index++;
        var health = Patch.Health(area);
        if (health <= 0f) continue;
        var bounds = Patch.Bound(area);
        var pos = Patch.Pos(bounds);
        var size = Patch.Size(bounds);
        var rot = Patch.Rot(bounds);
        var mask = Patch.RayMask(obj);
        int num = Physics.OverlapBoxNonAlloc(obj.transform.position + pos, size, tempColliders, rot, mask);
        var areaCollider = Patch.Collider(area);
        for (int i = 0; i < num; i++) {
          var collider = tempColliders[i];
          if (!(collider == areaCollider) && !(collider.attachedRigidbody != null) && !collider.isTrigger) {
            var componentInParent = collider.gameObject.GetComponentInParent<IDestructible>();
            if ((object)componentInParent == obj) continue;
            var supported = Patch.MineRock5_GetSupport(obj, collider);
            if (supported) {
              supportedCount++;
              var box = Drawer.DrawBox(obj, Settings.Color, Settings.LineWidth, "", pos, size);
              Drawer.AddTag(box, Constants.SupportTag);
              Drawer.AddText(box, "Index: " + Format.Int(index), "Size: " + Format.Coordinates(2 * size, "F1"));
              break;
            }
          }
        }
      }
      if (supportedCount > Settings.MaxAmount) Drawer.Remove(obj, Constants.SupportTag);
    }
  }
  [HarmonyPatch(typeof(MineRock5), "UpdateSupport")]
  public class MineRock5_Support {
    public static void Postfix(MineRock5 __instance) => Visual.DrawSupport(__instance);
  }
}
