using System.Collections.Generic;
using System.Linq;
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
    public bool IsSupported => tag == Tag.MineRock;
    public void SetSupportedAsCritical() {
      if (tag == Tag.MineRock) tag = Tag.CriticalMineRock;
    }
    public void SetMineRockAsCritical() {
      if (tag == Tag.MineRock || tag == Tag.ClearedMineRock) tag = Tag.CriticalMineRock;
    }

    public void Draw() {
      var color =
        tag == Tag.CriticalMineRock ? Settings.CriticalMineRockColor :
        tag == Tag.MineRock ? Settings.MineRockColor :
        tag == Tag.ClearedMineRock ? Settings.ClearedMineRockColor :
        Settings.DestructibleColor;
      var box = Drawer.DrawBox(tag, obj, color, Settings.LineWidth, position, size);
      Drawer.AddText(box, title, text);
    }
  }
  ///<summary>Code to check which parts are supported or supporting.</summary>
  public class SupportChecker {

    private static Collider[] tempColliders = new Collider[128];
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
    ///<summary>Returns bounding boxes of supporting and supported parts.</summary>
    public static IList<Box> CalculateBoundingBoxes(MineRock5 obj, ISet<int> supportedAreas) {
      var areas = Patch.HitAreas(obj);
      var index = -1;
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
          if (supported) supportedAreas.Add(index);
          var tag = supported ? Tag.MineRock : Tag.ClearedMineRock;
          boxes.Add(new Box(tag, obj.gameObject, pos, size, "Size: " + Format.Coordinates(2 * size, "F1"), "Index: " + Format.Int(index)));
        }
      }
      return boxes;
    }
  }
}
