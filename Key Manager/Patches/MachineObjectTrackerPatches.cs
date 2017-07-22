using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace spaar.Mods.KeyManager.Patches
{
  public class MachineObjectTrackerPatches
  {
    [HarmonyPatch(typeof(MachineObjectTracker), "CreateNewMachine")]
    class CreateNewMachine
    {
      static void Prefix()
      {
        // Reset key groups when a new machine is created
        
        // The patch will be run even if the mod was unloaded,
        // make sure to only call into the mod if it's properly loaded
        var go = GameObject.Find("Key Manager");
        if (go != null)
        {
          go.GetComponent<KeyManagerInterface>().KeyManager.ResetKeyGroups();
        }
      }
    }
  }
}
