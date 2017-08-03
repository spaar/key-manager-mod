using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyGroup
  {
    public string Name { get; set; }
    public Dictionary<KeyCode, List<Keybinding>> Keybindings { get; private set; }

    public KeyGroup(string name)
    {
      Name = name;

      Keybindings = new Dictionary<KeyCode, List<Keybinding>>();
    }

    /*private bool NoBindingsAssigned()
    {
      return Keybindings.Values.Aggregate(0, (count, list) => count + list.Count) == 0;
    }

    private bool NoKeysSpecified()
    {
      return Keybindings.Keys.All(k => k.KeyCode == KeyCode.None);
    }*/

    public void AddKeybinding(KeyCode key, Keybinding binding)
    {
      if (Keybindings.ContainsKey(key))
      {
        Keybindings[key].Add(binding);
      }
      else
      {
        Keybindings.Add(key, new List<Keybinding> { binding });
      }
    }

    public void AddKeybinding(Keybinding binding)
    {
      AddKeybinding(binding.Block.Keys[binding.MKeyIndex].KeyCode[binding.KeyCodeIndex], binding);
    }

    public void AddKeybinding(BlockBehaviour block, int mKeyIndex, int keyCodeIndex)
    {
      AddKeybinding(new Keybinding(block, mKeyIndex, keyCodeIndex));
    }

    /// <summary>
    /// Adds Keybindings for all controls that have at least one of the keys of this group assigned to them at any position.
    /// If the control has any keybinding added, one will be added for each key of this group.
    /// If the control already has a key, a Keybinding referencing the index of it will be added.
    /// If the group has keys that are currently not on the control, they will be added.
    /// </summary>
    public void AddAllWithKeys()
    {
      foreach (var block in Machine.Active().BuildingBlocks)
      {
        for (int i = 0; i < block.Keys.Count; i++)
        {
          var blockKeyCodes = block.Keys[i].KeyCode;
          // Find any keys present in the group and on the block.
          var intersection = blockKeyCodes.Intersect(Keybindings.Keys).ToList();
          if (intersection.Any())
          {
            // Add keys to the block that are on the group but not on the block.
            blockKeyCodes.AddRange(Keybindings.Keys.Where(k => !intersection.Contains(k)));

            // Add Keybindings referecing the keys on the block at their correct indices.
            foreach (var key in intersection)
            {
              AddKeybinding(key, new Keybinding(block, i, blockKeyCodes.IndexOf(key)));
            }
          }
        }
      }
    }

    public void RemoveKeybinding(KeyCode key, Keybinding keybinding)
    {
      if (Keybindings.ContainsKey(key))
      {
        Keybindings[key].Remove(keybinding);
      }
    }

    public void RemoveAllBindingsWithBlock(BlockBehaviour block)
    {
      foreach (var key in Keybindings.Keys)
      {
        Keybindings[key].RemoveAll(binding => binding.Block == block);
      }
    }

    public void RemoveAllBindingsWithoutBlock()
    {
      RemoveAllBindingsWithBlock(null);
    }

    public void RemoveAllBindingsWithKey(KeyCode key)
    {
      if (!Keybindings.ContainsKey(key)) return;

      // Remove bindings from group.
      var bindings = Keybindings[key];
      Keybindings.Remove(key);

      // Remove corresponding key from the blocks.
      foreach (var binding in bindings)
      {
        binding.Key.KeyCode.RemoveAt(binding.KeyCodeIndex);

        // Don't remove the last key from a block, could break assumptions in the game.
        if (binding.Key.KeyCode.Count == 0)
        {
          binding.Key.KeyCode.Add(key);
        }
      }
    }

    /// <summary>
    /// Adds keybindings for newKey to every MKey that currently has a keybinding associated with this group.
    /// If the corresponding MKey already has the key assigned to it, it will be taken over, otherwise a new one will be added.
    /// NOP if this group already has newKey.
    /// </summary>
    /// <param name="newKey"></param>
    public void AddKey(KeyCode newKey)
    {
      if (Keybindings.ContainsKey(newKey)) return;

      Keybindings.Add(newKey, new List<Keybinding>());

      var blocks = AllAssignedBlocks();
      var mKeys = AllAssignedMKeys();
      foreach (var mKey in mKeys)
      {
        if (!mKey.KeyCode.Contains(newKey))
        {
          mKey.AddKey(newKey);
        }

        var block = blocks.Find(b => b.Keys.Contains(mKey));
        AddKeybinding(block, block.Keys.IndexOf(mKey), mKey.KeyCode.IndexOf(newKey));
      }
    }

    /// <summary>
    /// Changes all Keybindings currently assigned to previousKey to newKey.
    /// NOP if there are no keybindings assigned to previousKey.
    /// If there are already bindings associated with newKey present,
    /// all bindings of previousKey will be deleted and the KeyCode be removed from their blocks.
    /// </summary>
    /// <param name="previousKey"></param>
    /// <param name="newKey"></param>
    public void ChangeKey(KeyCode previousKey, KeyCode newKey)
    {
      if (!Keybindings.ContainsKey(previousKey)) return;

      // If the key is already present, delete associated bindings and keys.
      // Don't delete the last key present though.
      if (Keybindings.ContainsKey(newKey) && Keybindings.Keys.Count > 1)
      {
        RemoveAllBindingsWithKey(previousKey);
        return;
      }

      // Change what key the bindings are assigned to.
      var bindings = Keybindings[previousKey];
      Keybindings.Remove(previousKey);
      Keybindings[newKey] = bindings;

      // Set the new key on the actual block controls.
      bindings.ForEach(b =>
      {
        b.Key.AddOrReplaceKey(b.KeyCodeIndex, newKey);
      });
    }

    public bool HasBlock(BlockBehaviour block)
    {
      return Keybindings.Any(pair => pair.Value.Any(binding => binding.Block == block));
    }

    // TODO: Consider converting these to properties
    public List<BlockBehaviour> AllAssignedBlocks()
    {
      return new HashSet<BlockBehaviour>(Keybindings.Values.SelectMany(l => l.Select(b => b.Block))).ToList();
    }

    public List<MKey> AllAssignedMKeys()
    {
      return new HashSet<MKey>(Keybindings.Values.SelectMany(l => l.Select(b => b.Key))).ToList();
    }

    public bool HasNoBindings()
    {
      return Keybindings.Values.All(l => l.Count == 0);
    }

    public string KeyString()
    {
      return string.Join(", ", Keybindings.Keys.Select(k => k.ToString()).ToArray());
    }
  }
}