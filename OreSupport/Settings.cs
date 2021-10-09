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
    public static ConfigEntry<float> configRefreshInterval;
    public static float RefreshInterval => Math.Max(1f, configRefreshInterval.Value);
    public static ConfigEntry<int> configMaxAmount;
    public static int MaxAmount => configMaxAmount.Value;
    public static ConfigEntry<int> configMinSize;
    public static int MinSize => configMinSize.Value;
    public static ConfigEntry<string> configMineRockColor;
    public static Color MineRockColor => ParseColor(configMineRockColor.Value);
    public static ConfigEntry<string> configClearedMineRockColor;
    public static Color ClearedMineRockColor => ParseColor(configClearedMineRockColor.Value);
    public static ConfigEntry<string> configDestructibleColor;
    public static Color DestructibleColor => ParseColor(configDestructibleColor.Value);
    public static void Init(ConfigFile config) {
      var section = "General";
      configRefreshInterval = config.Bind(section, "Refresh interval", 5f, new ConfigDescription("How often the support is checked.", new AcceptableValueRange<float>(1f, 60f)));
      configMaxAmount = config.Bind(section, "Max pieces", 50, new ConfigDescription("Max amount of boxes before showing any (0 to disable).", new AcceptableValueRange<int>(0, 100)));
      configMinSize = config.Bind(section, "Min size", 10, new ConfigDescription("Minimum amount of pieces to display any boxes.", new AcceptableValueRange<int>(0, 150)));
      configLineWidth = config.Bind(section, "Line width", 2f, new ConfigDescription("Line width of the bounding boxes.", new AcceptableValueRange<float>(1f, 100f)));
      configLineWidth.SettingChanged += (s, e) => {
        Drawer.SetLineWidth(Tag.MineRock, LineWidth);
        Drawer.SetLineWidth(Tag.Destructible, LineWidth);
      };
      configMineRockColor = config.Bind(section, "Supported color", "red", "Color of supported pieces.");
      configMineRockColor.SettingChanged += (s, e) => {
        Drawer.SetColor(Tag.MineRock, MineRockColor);
      };
      configClearedMineRockColor = config.Bind(section, "Unsupported color", "green", "Color of pieces that are no longer supported.");
      configClearedMineRockColor.SettingChanged += (s, e) => {
        Drawer.SetColor(Tag.ClearedMineRock, ClearedMineRockColor);
      };
      configDestructibleColor = config.Bind(section, "Support color", "yellow", "Color of supporting objects.");
      configDestructibleColor.SettingChanged += (s, e) => {
        Drawer.SetColor(Tag.Destructible, DestructibleColor);
      };
    }
  }
}
