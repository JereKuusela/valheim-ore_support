using System;
using BepInEx.Configuration;
using UnityEngine;

namespace OreSupport {
  public partial class Settings {
    private static Color ParseColor(string color) {
      if (ColorUtility.TryParseHtmlString(color, out var parsed)) return parsed;
      return Color.white;
    }
    public static ConfigEntry<float> configLineWidth;
    public static float LineWidth => Math.Max(0.01f, configLineWidth.Value);
    public static ConfigEntry<int> configMaxAmount;
    public static int MaxAmount => configMaxAmount.Value;
    public static ConfigEntry<int> configMinSize;
    public static int MinSize => configMinSize.Value;
    public static ConfigEntry<string> configColor;
    public static Color Color => ParseColor(configColor.Value);
    public static void Init(ConfigFile config) {
      var section = "General";
      configMaxAmount = config.Bind(section, "Max pieces", 50, "Max amount of boxes before showing any (0 to disable).");
      configMinSize = config.Bind(section, "Min size", 10, "Minimum amount of parts to display any boxes.");
      configLineWidth = config.Bind(section, "Line width", 0.02f, "Line width of the bounding boxes.");
      configLineWidth.SettingChanged += (s, e) => {
        Drawer.SetLineWidth(Constants.SupportTag, LineWidth);
      };
      configColor = config.Bind(section, "Color", "red", "");
      configColor.SettingChanged += (s, e) => {
        Drawer.SetColor(Constants.SupportTag, Color);
      };
    }
  }
}
