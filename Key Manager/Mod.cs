using System;
using spaar.ModLoader;
using UnityEngine;

namespace Key_Manager
{

  // If you need documentation about any of these values or the mod loader
  // in general, take a look at https://spaar.github.io/besiege-modloader.

  public class YourMod : Mod
  {
    public override string Name { get; } = "<placeholder>";
    public override string DisplayName { get; } = "<placeholder>";
    public override string Author { get; } = "<placeholder>";
    public override Version Version { get; } = new Version(1, 0, 0);

    public override void OnLoad()
    {
      // Your initialization code here
    }

    public override void OnUnload()
    {
      // Your code here
      // e.g. save configuration, destroy your objects if CanBeUnloaded is true etc
    }
  }
}
