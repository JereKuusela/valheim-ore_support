using BepInEx;
using HarmonyLib;
using UnityEngine;
namespace OreSupport;
[BepInPlugin(GUID, NAME, VERSION)]
public class OreSupport : BaseUnityPlugin
{
  const string GUID = "ore_support";
  const string NAME = "Ore Support";
  const string VERSION = "1.9";
  public void Awake()
  {
    Settings.Init(Config);
    Harmony harmony = new(GUID);
    harmony.PatchAll();
    InitCommands();
  }
#pragma warning disable IDE0051
  void LateUpdate()
  {
    SupportUpdater.RefreshSupport(Time.deltaTime);
  }
#pragma warning restore IDE0051

  private static void InitCommands()
  {
    new Terminal.ConsoleCommand("ore_support", "Toggles mine rock support visibility.", (args) =>
    {
      Settings.configEnable.Value = !Settings.Enable;
      args.Context.AddString("Ore support " + (Settings.Enable ? "enabled" : "disabled") + ".");
      SupportUpdater.UpdateSupport();
    });
  }
}
