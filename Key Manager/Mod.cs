using System;
using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.KeyManager
{

  public class KeyManagerMod : ModLoader.Mod
  {
    public override string Name { get; } = "keyManager";
    public override string DisplayName { get; } = "Key Manager";
    public override string Author { get; } = "spaar";
    public override Version Version { get; } = new Version(0, 0, 1);

    public override void OnLoad()
    {
      Textures.Init();

      var manager = new KeyManager();
      KeyManagerInterface.Instance.KeyManager = manager;
    }

    public override void OnUnload()
    {
      GameObject.Destroy(KeyManagerInterface.Instance);
    }
  }
}
