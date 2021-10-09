using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace OreSupport {
  [BepInPlugin("valheim.jerekuusela.ore_support", "OreSupport", "1.1.0.0")]
  public class OreSupport : BaseUnityPlugin {
    public void Awake() {
      Settings.Init(Config);
      var harmony = new Harmony("valheim.jerekuusela.ore_support");
      harmony.PatchAll();
    }
    private static float timer = 0;
    private static bool IsValid(MonoBehaviour obj) {
      if (!obj) return false;
      var nView = Patch.Nview(obj);
      if (!nView) return false;
      return nView.IsValid();
    }
    void LateUpdate() {
      timer += Time.deltaTime;
      if (timer >= Settings.RefreshInterval) {
        timer -= Settings.RefreshInterval;
        if (!IsValid(Visual.Tracked)) Visual.Tracked = null;
        if (Visual.Tracked) Visual.DrawSupport(Visual.Tracked, true);
      }
    }
  }
}
