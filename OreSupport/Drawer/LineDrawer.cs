using UnityEngine;
namespace OreSupport;
public partial class Drawer {
  private static void AddBoxCollider(GameObject obj, Vector3 center, Vector3 size) {
    var collider = obj.AddComponent<BoxCollider>();
    collider.isTrigger = true;
    collider.center = center;
    collider.size = size;
  }
  ///<summary>Creates a line.</summary>
  private static GameObject DrawLineSub(GameObject obj, Vector3 start, Vector3 end, Color color, float width) {
    var renderer = CreateRenderer(obj, color, width);
    renderer.SetPosition(0, start);
    renderer.SetPosition(1, end);
    return obj;
  }
  ///<summary>Creates a renderer with a vertical line (relative to the object).</summary>
  public static GameObject DrawBox(string tag, MonoBehaviour parent, Color color, float width, Vector3 center, Vector3 extents) {
    return DrawBox(tag, parent.gameObject, color, width, center, extents);
  }
  ///<summary>Creates a renderer with a vertical line (relative to the object).</summary>
  public static GameObject DrawBox(string tag, GameObject parent, Color color, float width, Vector3 center, Vector3 extents) {
    var corners = new Vector3[] {
        new (center.x - extents.x, center.y - extents.y, center.z - extents.z),
        new (center.x - extents.x, center.y - extents.y, center.z + extents.z),
        new (center.x - extents.x, center.y + extents.y, center.z - extents.z),
        new (center.x - extents.x, center.y + extents.y, center.z + extents.z),
        new (center.x + extents.x, center.y - extents.y, center.z - extents.z),
        new (center.x + extents.x, center.y - extents.y, center.z + extents.z),
        new (center.x + extents.x, center.y + extents.y, center.z - extents.z),
        new (center.x + extents.x, center.y + extents.y, center.z + extents.z),
    };
    var obj = CreateObject(parent, tag, true);
    for (var i = 0; i < corners.Length; i++) {
      var start = corners[i];
      for (var j = i + 1; j < corners.Length; j++) {
        var end = corners[j];
        var same = 0;
        if (start.x == end.x) same++;
        if (start.y == end.y) same++;
        if (start.z == end.z) same++;
        if (same != 2) continue;
        DrawLineSub(CreateObject(obj), corners[i], corners[j], color, width);
      }
    }
    Drawer.AddBoxCollider(obj, center, extents * 2.0f);
    return obj;
  }
}
