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
    private static IEnumerable<Collider> Filter(IEnumerable<Collider> colliders, MineRock5 obj, MineRock5.HitArea area) {
      return colliders.Where(collider => {
        if (collider == area.m_collider || collider.attachedRigidbody != null || collider.isTrigger) return false;
        var destructible = collider.gameObject.GetComponentInParent<IDestructible>();
        if ((object)destructible == obj) return false;
        return true;
      });
    }
    private static bool CheckColliders(IEnumerable<Collider> colliders, MineRock5 obj, List<Box> boxes, int groundLayer) {
      if (Settings.ShowSupporting) {
        var supportingColliders = colliders.Where(obj.GetSupport);
        var objectColliders = supportingColliders.Where(collider => collider.gameObject.layer != groundLayer);
        foreach (var collider in objectColliders)
          boxes.Add(new Box(Tag.Destructible, obj.gameObject, collider.bounds.center - obj.transform.position, collider.bounds.extents, "Supports mine rock", ""));
        return supportingColliders.Count() > 0;
      } else {
        return colliders.Any(obj.GetSupport);
      }
    }
    ///<summary>Returns bounding boxes of supporting and supported parts.</summary>
    public static IList<Box> CalculateBoundingBoxes(MineRock5 obj, ISet<int> supportedAreas) {
      var index = -1;
      var boxes = new List<Box>();
      foreach (var area in obj.m_hitAreas) {
        index++;
        var health = area.m_health;
        if (health <= 0f) continue;
        var bounds = area.m_bound;
        var pos = bounds.m_pos;
        var size = bounds.m_size;
        var rot = bounds.m_rot;
        var num = Physics.OverlapBoxNonAlloc(obj.transform.position + pos, size, tempColliders, rot, MineRock5.m_rayMask);
        var colliders = Filter(tempColliders.Take(num), obj, area);
        var supported = CheckColliders(colliders, obj, boxes, MineRock5.m_groundLayer);
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
