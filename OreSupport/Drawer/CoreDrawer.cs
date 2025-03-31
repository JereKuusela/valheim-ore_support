using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OreSupport;


public partial class Drawer
{
  private static readonly Texture2D Texture = new(1, 1);
  private static Shader LineShader => lineShader
    ??= Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(shader => shader.name == shaderName) ?? Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(shader => shader.name == "Sprites/Default") ?? throw new Exception("Shader not found.");
  private static Shader? lineShader;
  private static string shaderName = "Sprites/Default";
  public static void SetShader(string name)
  {
    shaderName = name;
    lineShader = null;
    materials.Clear();
    foreach (var obj in Utils.GetVisualizations()) ChangeColor(obj.gameObject);
  }
  private static readonly Dictionary<string, Color> colors = [];
  public static Color GetColor(string tag) => colors.ContainsKey(tag) ? colors[tag] : Color.white;
  private static void ChangeColor(GameObject obj)
  {
    var renderer = obj.GetComponent<LineRenderer>();
    if (renderer) renderer.sharedMaterial = GetMaterial(GetColor(obj.name));
  }
  public static void Init()
  {
    Texture.SetPixel(0, 0, Color.gray);
  }

  ///<summary>Creates the base object for drawing.</summary>
  private static GameObject CreateObject(GameObject parent, string tag = "", bool fixRotation = false)
  {
    GameObject obj = new()
    {
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
  private static readonly Dictionary<Color, Material> materials = [];
  private static Material GetMaterial(Color color)
  {
    if (materials.ContainsKey(color)) return materials[color];
    var material = new Material(LineShader);
    material.SetColor("_Color", color);
    material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Subtract);
    material.SetTexture("_MainTex", Texture);
    materials[color] = material;
    return material;
  }
  ///<summary>Creates the line renderer object.</summary>
  private static LineRenderer CreateRenderer(GameObject obj, Color color, float width)
  {
    var renderer = obj.AddComponent<LineRenderer>();
    renderer.useWorldSpace = false;
    renderer.sharedMaterial = GetMaterial(color);
    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    renderer.widthMultiplier = width / 100f;
    return renderer;
  }
  ///<summary>Changes object color.</summary>
  private static void ChangeColor(GameObject obj, Color color)
  {
    foreach (var renderer in obj.GetComponentsInChildren<LineRenderer>(true))
      renderer.sharedMaterial = GetMaterial(color);
  }
  ///<summary>Changes object line width.</summary>
  private static void ChangeLineWidth(GameObject obj, float width)
  {
    foreach (var renderer in obj.GetComponentsInChildren<LineRenderer>(true))
      renderer.widthMultiplier = width / 100f;
  }
  ///<summary>Adds a custom text with a title and text to a given object.</summary>
  public static void AddText(GameObject obj, string title, string text)
  {
    var component = obj.AddComponent<StaticText>();
    component.text = text;
    component.title = title;
  }
  ///<summary>Adds a tag to a given renderer so it can be found later.</summary>
  public static void AddTag(GameObject obj, string tag)
  {
    obj.AddComponent<Visualization>().Tag = tag;
  }
  ///<summary>Removes visuals with a given tag.</summary>
  public static void Remove(MonoBehaviour parent, string tag)
  {
    foreach (var obj in parent.GetComponentsInChildren<Visualization>(true))
    {
      if (obj.Tag == tag) UnityEngine.Object.Destroy(obj.gameObject);
    }
  }
  ///<summary>Sets colors to visuals with a given tag.</summary>
  public static void SetColor(string tag, Color color)
  {
    colors[tag] = color;
    foreach (var obj in Utils.GetVisualizations(tag))
    {
      ChangeColor(obj.gameObject, color);
    }
  }
  ///<summary>Sets line width to visuals with a given tag.</summary>
  public static void SetLineWidth(string tag, float width)
  {
    foreach (var obj in Utils.GetVisualizations(tag))
    {
      ChangeLineWidth(obj.gameObject, width);
    }
  }
}
