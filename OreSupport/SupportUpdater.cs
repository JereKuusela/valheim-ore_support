using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace OreSupport {
  public class SupportUpdater {
    public static MineRock5 Tracked = null;
    private static float timer = 0;
    private static ISet<int> supportedAreas = new HashSet<int>();
    private static void Clear(MineRock5 obj) {
      if (!obj) return;
      Drawer.Remove(obj, Tag.MineRock);
      Drawer.Remove(obj, Tag.ClearedMineRock);
      Drawer.Remove(obj, Tag.Destructible);
      Drawer.Remove(obj, Tag.CriticalMineRock);
    }
    private static bool IsValid(MineRock5 obj) {
      if (!obj) return false;
      var nView = obj.m_nview;
      if (!nView) return false;
      return nView.IsValid();
    }
    public static void DrawSupport(MineRock5 obj) {
      if (obj != Tracked) {
        Clear(Tracked);
        Tracked = obj;
      }
      supportedAreas.Clear();
      UpdateSupport();
    }
    public static void RefreshSupport(float delta) {
      timer += delta;
      if (timer >= Settings.RefreshInterval)
        UpdateSupport();
    }
    public static void UpdateSupport() {
      timer = 0f;
      Clear(Tracked);
      if (!IsValid(Tracked)) {
        Tracked = null;
        return;
      }
      if (!Settings.Enable) return;
      if (Settings.LineWidth == 0 || Settings.MaxBoxes == 0) return;
      var areas = Tracked.m_hitAreas;
      if (areas.Count() < Settings.MinSize) return;
      if (areas.Where(area => area.m_health > 0f).Count() > Settings.MaxParts) return;
      var boxes = SupportChecker.CalculateBoundingBoxes(Tracked, supportedAreas);
      if (boxes.Count > Settings.MaxBoxes) return;
      var onlySupport = boxes.Count(box => box.IsSupported) == 1;
      var noSupport = boxes.Count(box => box.IsSupported) == 0;
      if (noSupport)
        foreach (var box in boxes) box.SetMineRockAsCritical();
      else if (onlySupport)
        foreach (var box in boxes) box.SetSupportedAsCritical();
      foreach (var box in boxes) box.Draw();
    }
  }
  // Starts tracking the support when hitting a mine rock.
  [HarmonyPatch(typeof(MineRock5), "RPC_Damage")]
  public class MineRock5_Damage {
    public static void Postfix(MineRock5 __instance) {
      if (SupportUpdater.Tracked == __instance) return;
      if (!__instance.m_haveSetupBounds) {
        __instance.SetupColliders();
        __instance.m_haveSetupBounds = true;
      }
      SupportUpdater.DrawSupport(__instance);
    }
  }
  // Update the state.
  [HarmonyPatch(typeof(MineRock5), "UpdateSupport")]
  public class MineRock5_Support {
    public static void Postfix(MineRock5 __instance) => SupportUpdater.DrawSupport(__instance);
  }

}
