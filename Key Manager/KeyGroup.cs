﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyGroup
  {
    public string Name { get; set; }
    public KeyCode Key { get; private set; }

    private List<Keybinding> bindings;

    public IList<Keybinding> AssignedBindings
    {
      get { return bindings.AsReadOnly(); }
    }

    public KeyGroup(string name, KeyCode key)
    {
      Name = name;
      Key = key;
      bindings = new List<Keybinding>();
    }

    public void AddKeybinding(Keybinding binding)
    {
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

    public void SetKey(KeyCode keyCode)
    {
      Key = keyCode;

      foreach (var binding in bindings)
      {
        binding.Key.AddOrReplaceKey(0, keyCode);
      }
    }

    public bool HasBlock(BlockBehaviour block)
    {
      return bindings.Find(b => b.Block == block) != null;
    }
  }
}