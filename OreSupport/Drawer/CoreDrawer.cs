using System;
using HarmonyLib;
using UnityEngine;
namespace OreSupport;
[HarmonyPatch(typeof(Player), nameof(Player.UpdateHover))]
public class Player_AddHoverForVisuals {
  /// <summary>Extra hover search for drawn objects if no other hover object.</summary>
  public static void Postfix(ref GameObject ___m_hovering, ref GameObject ___m_hoveringCreature) {
    if (___m_hovering || ___m_hoveringCreature || !Settings.Enable || !SupportUpdater.Tracked) return;
    var distance = 100f;
    var mask = LayerMask.GetMask(Constants.TriggerLayer);
    var hits = Physics.RaycastAll(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, distance, mask);
    // Reverse search is used to find edge when inside colliders.
    var reverseHits = Physics.RaycastAll(GameCamera.instance.transform.position + GameCamera.instance.transform.forward * distance, -GameCamera.instance.transform.forward, distance, mask);
    hits = hits.AddRangeToArray(reverseHits);
    Array.Sort<RaycastHit>(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
    foreach (var hit in hits) {
      if (hit.collider.GetComponent<Visualization>() != null) {
        ___m_hovering = hit.collider.gameObject;
        return;
      }
    }
  }
}

/// <summary>Custom text that also shows the title.</summary>
public class StaticText : MonoBehaviour, Hoverable {
  public string GetHoverText() => Format.String(title) + "\n" + text;
  public string GetHoverName() => title;
  public string title = "";
  public string text = "";
}
/// <summary>Provides a way to distinguish renderers.</summary>
public class Visualization : MonoBehaviour {
  public string customTag = "";
}

public partial class Drawer : Component {
  ///<summary>Creates the base object for drawing.</summary>
  private static GameObject CreateObject(GameObject parent, string tag = "", bool fixRotation = false) {
    GameObject obj = new() {
      layer = LayerMask.NameToLayer(Constants.TriggerLayer),
      name = tag
    };
    obj.transform.parent = parent.transform;
    obj.transform.localPosition = Vector3.zero;
    if (!fixRotation)
      obj.transform.localRotation = Quaternion.identity;
    if (tag != "")
      AddTag(obj, tag);
    return obj;
  }
  ///<summary>Creates the line renderer object.</summary>
  private static LineRenderer CreateRenderer(GameObject obj, Color color, float width) {
    var renderer = obj.AddComponent<LineRenderer>();
    renderer.useWorldSpace = false;
    Material material = new(Shader.Find("Particles/Standard Unlit2"));
    material.SetColor("_Color", color);
    material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Subtract);
    Texture2D texture = new(1, 1);
    texture.SetPixel(0, 0, Color.gray);
    material.SetTexture("_MainTex", texture);
    renderer.material = material;
    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    renderer.widthMultiplier = width / 100f;
    return renderer;
  }
  ///<summary>Changes object color.</summary>
  private static void ChangeColor(GameObject obj, Color color) {
    foreach (var renderer in obj.GetComponentsInChildren<LineRenderer>(true))
      renderer.material.SetColor("_Color", color);
  }
  ///<summary>Changes object line width.</summary>
  private static void ChangeLineWidth(GameObject obj, float width) {
    foreach (var renderer in obj.GetComponentsInChildren<LineRenderer>(true))
      renderer.widthMultiplier = width / 100f;
  }
  ///<summary>Adds a custom text with a title and text to a given object.</summary>
  public static void AddText(GameObject obj, string title, string text) {
    var component = obj.AddComponent<StaticText>();
    component.text = text;
    component.title = title;
  }
  ///<summary>Adds a tag to a given renderer so it can be found later.</summary>
  public static void AddTag(GameObject obj, string tag) {
    obj.AddComponent<Visualization>().customTag = tag;
  }
  ///<summary>Removes visuals with a given tag.</summary>
  public static void Remove(MonoBehaviour parent, string tag) {
    foreach (var obj in parent.GetComponentsInChildren<Visualization>(true)) {
      if (obj.customTag == tag) Destroy(obj.gameObject);
    }
  }
  ///<summary>Sets colors to visuals with a given tag.</summary>
  public static void SetColor(string tag, Color color) {
    foreach (var customTag in Resources.FindObjectsOfTypeAll<Visualization>()) {
      if (customTag.customTag == tag)
        ChangeColor(customTag.gameObject, color);
    }
  }
  ///<summary>Sets line width to visuals with a given tag.</summary>
  public static void SetLineWidth(string tag, float width) {
    foreach (var customTag in Resources.FindObjectsOfTypeAll<Visualization>()) {
      if (customTag.customTag == tag)
        ChangeLineWidth(customTag.gameObject, width);
    }
  }
}
