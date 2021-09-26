using BepInEx;
using HarmonyLib;

namespace OreSupport {
  [BepInPlugin("valheim.jerekuusela.ore_support", "OreSupport", "1.0.0.0")]
  public class OreSupport : BaseUnityPlugin {
    public void Awake() {
      Settings.Init(Config);
      var harmony = new Harmony("valheim.jerekuusela.ore_support");
      harmony.PatchAll();
    }
  }
}
