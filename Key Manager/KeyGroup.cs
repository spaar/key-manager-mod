using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyGroup
  {
    public string Name { get; set; }
    public List<KeyCode> Keys { get; private set; }

    private List<Keybinding> bindings;

    public IList<Keybinding> AssignedBindings
    {
      get { return bindings.AsReadOnly(); }
    }

    public KeyGroup(string name, List<KeyCode> keys)
    {
      Name = name;
      Keys = keys;
      Keys.Add(KeyCode.None);
      bindings = new List<Keybinding>();
    }

    public KeyGroup(string name, KeyCode key) : this(name, new List<KeyCode>() {key})
    {
      
    }

    public void AddKeybinding(Keybinding binding)
    {
      // If the group does not yet have a key, use the one of the first assigned control
      if (Keys[0] == KeyCode.None)
      {
        Keys[0] = binding.Key.KeyCode[0];
      }

      bindings.Add(binding);
    }

    public void AddKeybinding(BlockBehaviour block, int keyIndex)
    {
      AddKeybinding(new Keybinding(block, keyIndex));
    }

    public void AddAllWithKey(KeyCode key)
    {
      foreach (var block in Machine.Active().BuildingBlocks)
      {
        for (int i = 0; i < block.Keys.Count; i++)
        {
          if (block.Keys[i].KeyCode[0] == key)
          {
            AddKeybinding(block, i);
            break;
          }
        }
      }
    }

    public void RemoveKeybinding(Keybinding keybinding)
    {
      bindings.Remove(keybinding);
    }

    public void RemoveKeybinding(BlockBehaviour block)
    {
      RemoveKeybinding(bindings.Find(b => b.Guid == block.Guid));
    }

    // It is not allowed to call SetKey with an index >= Keys.Count
    public void SetKey(int index, KeyCode keyCode)
    {
      if (index >= Keys.Count)
      {
        throw new ArgumentException("Index is too high.");
      }
      else if (index == Keys.Count - 1)
      {
        Keys[index] = keyCode;
        Keys.Add(KeyCode.None);
      }
      else
      {
        Keys[index] = keyCode;
      }

      foreach (var binding in bindings)
      {
        binding.Key.AddOrReplaceKey(index, keyCode);
      }
    }

    public bool HasBlock(BlockBehaviour block)
    {
      return bindings.Find(b => b.Block == block) != null;
    }
  }
}