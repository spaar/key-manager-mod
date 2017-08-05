using System;
using System.Reflection;
using Harmony;
using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.KeyManager
{

  public class KeyManagerMod : ModLoader.Mod
  {
    public override string Name { get; } = "keyManager";
    public override string DisplayName { get; } = "Key Manager";
    public override string Author { get; } = "spaar";
    public override Version Version { get; } = new Version(1, 1, 0);

    // Technically the Harmony patches can't be undone, but they're
    // written in such a way that they don't cause any harm if the mod is unloaded at runtime.
    public override bool CanBeUnloaded { get; } = true;
    // Risk that it will break in some way on other versions is reasonably high because of the patches.
    public override string BesiegeVersion { get; } = "v0.45a";

    public override void OnLoad()
    {
      // Load all patches (located in the the Patches folder)
      var harmony = HarmonyInstance.Create("spaar.Mods.KeyManager");
      harmony.PatchAll(Assembly.GetExecutingAssembly());
      
      // Load all GUI textures
      Resources.Init();

      // Create key manager
      var manager = new KeyManager();
      KeyManagerInterface.Instance.KeyManager = manager;

      // Allow showing the example machine again even after dismissing it
      Commands.RegisterCommand("resetExampleMachine", (args, nArgs) =>
      {
        Configuration.SetBool("dismissed-example-dialog", false);
        return "Reset.";
      }, "Have the Key Manager offer to show the example machine again.");
    }

    public override void OnUnload()
    {
      KeyManagerInterface.Instance.OnUnload();
      GameObject.Destroy(KeyManagerInterface.Instance);
    }
  }
}
