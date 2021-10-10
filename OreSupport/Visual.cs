using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace OreSupport {
  ///<summary>Performance: Store boxes to delay drawing since they may be hidden too.</summary>
  public class Box {
    public Box(string tag, GameObject obj, Vector3 pos, Vector3 size, string title, string text = "") {
      this.tag = tag;
      this.obj = obj;
      this.position = pos;
      this.size = size;
      this.title = title;
      this.text = text;
    }
    private Vector3 position;
    private Vector3 size;
    private string title;
    private string text;
    private string tag;
    private GameObject obj;

    public void Draw() {
      var color = tag == Tag.MineRock ? Settings.MineRockColor : tag == Tag.ClearedMineRock ? Settings.ClearedMineRockColor : Settings.DestructibleColor;
      var box = Drawer.DrawBox(tag, obj, color, Settings.LineWidth, position, size);
      Drawer.AddText(box, title, text);
    }
  }
  public class Visual {
    public static MineRock5 Tracked = null;

    private static Collider[] tempColliders = new Collider[128];
    private static ISet<int> supportedAreas = new HashSet<int>();
    private static void Clear(MineRock5 obj) {
      if (!obj) return;
      Drawer.Remove(obj, Tag.MineRock);
      Drawer.Remove(obj, Tag.ClearedMineRock);
      Drawer.Remove(obj, Tag.Destructible);

    }
    private static IEnumerable<Collider> Filter(IEnumerable<Collider> colliders, MineRock5 obj, object area) {
      var areaCollider = Patch.Collider(area);
      return colliders.Where(collider => {
        if (collider == areaCollider || collider.attachedRigidbody != null || collider.isTrigger) return false;
        var destructible = collider.gameObject.GetComponentInParent<IDestructible>();
        if ((object)destructible == obj) return false;
        return true;
      });
    }
    private static bool CheckColliders(IEnumerable<Collider> colliders, MineRock5 obj, List<Box> boxes, int groundLayer) {
      if (Settings.ShowSupporting) {
        var supportingColliders = colliders.Where(collider => Patch.MineRock5_GetSupport(obj, collider));
        var objectColliders = supportingColliders.Where(collider => collider.gameObject.layer != groundLayer);
        foreach (var collider in objectColliders)
          boxes.Add(new Box(Tag.Destructible, obj.gameObject, collider.bounds.center - obj.transform.position, collider.bounds.extents, "Supports mine rock", ""));
        return supportingColliders.Count() > 0;
      } else {
        return colliders.Any(collider => Patch.MineRock5_GetSupport(obj, collider));
      }
    }

    public static void DrawSupport(MineRock5 obj, bool update) {
      Clear(Visual.Tracked);
      Visual.Tracked = obj;
      if (!update) supportedAreas.Clear();
      if (Settings.LineWidth == 0 || Settings.MaxBoxes == 0) return;
      var areas = Patch.HitAreas(obj);
      if (areas.Count() < Settings.MinSize) return;
      if (areas.Where(area => Patch.Health(area) > 0f).Count() > Settings.MaxParts) return;
      var index = -1;
      var supportedCount = 0;
      var groundLayer = Patch.GroundLayer(obj);
      var boxes = new List<Box>();
      foreach (var area in areas) {
        index++;
        var health = Patch.Health(area);
        if (health <= 0f) continue;
        var bounds = Patch.Bound(area);
        var pos = Patch.Pos(bounds);
        var size = Patch.Size(bounds);
        var rot = Patch.Rot(bounds);
        var mask = Patch.RayMask(obj);
        var num = Physics.OverlapBoxNonAlloc(obj.transform.position + pos, size, tempColliders, rot, mask);
        var colliders = Filter(tempColliders.Take(num), obj, area);
        var supported = CheckColliders(colliders, obj, boxes, groundLayer);
        var wasSupported = supportedAreas.Contains(index);
        if (supported || wasSupported) {
          var tag = supported ? Tag.MineRock : Tag.ClearedMineRock;
          boxes.Add(new Box(tag, obj.gameObject, pos, size, "Size: " + Format.Coordinates(2 * size, "F1"), "Index: " + Format.Int(index)));
        }
      }
      if (supportedCount > Settings.MaxBoxes) return;
      foreach (var box in boxes) box.Draw();
    }
  }
  [HarmonyPatch(typeof(MineRock5), "UpdateSupport")]
  public class MineRock5_Support {
    public static void Postfix(MineRock5 __instance) => Visual.DrawSupport(__instance, false);
  }

  [HarmonyPatch(typeof(MineRock5), "RPC_Damage")]
  public class MineRock5_Damage {
    public static void Postfix(MineRock5 __instance, ref bool ___m_haveSetupBounds) {
      if (Visual.Tracked == null) {
        if (!___m_haveSetupBounds) {
          Patch.MineRock5_SetupColliders(__instance);
          ___m_haveSetupBounds = true;
        }
        Visual.DrawSupport(__instance, false);
      }
    }
  }
}
