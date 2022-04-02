using BepInEx;
using HarmonyLib;
using UnityEngine;
namespace OreSupport;
[BepInPlugin("valheim.jerekuusela.ore_support", "OreSupport", "1.5.0.0")]
public class OreSupport : BaseUnityPlugin {
  public void Awake() {
    Settings.Init(Config);
    Harmony harmony = new("valheim.jerekuusela.ore_support");
    harmony.PatchAll();
    InitCommands();
  }
  void LateUpdate() {
    SupportUpdater.RefreshSupport(Time.deltaTime);
  }

  private static void InitCommands() {
    new Terminal.ConsoleCommand("ore_support", "Toggles mine rock support visibility.", (Terminal.ConsoleEventArgs args) => {
      Settings.configEnable.Value = !Settings.Enable;
      args.Context.AddString("Ore support " + (Settings.Enable ? "enabled" : "disabled") + ".");
      SupportUpdater.UpdateSupport();
    });
  }
}
