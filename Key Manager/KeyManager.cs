using System;
using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyManager
  {
    private List<KeyGroup> groups;
    public IList<KeyGroup> KeyGroups
    {
      get { return groups.AsReadOnly(); }
    }

    public KeyManager()
    {
      groups = new List<KeyGroup>();

      XmlSaver.OnSave += info =>
      {
        var machineData = info.MachineData;

        machineData.Write("keymanager-group-count", groups.Count);
        for (int i = 0; i < groups.Count; i++)
        {
          var groupTag = $"keymanager-group-{i}";
          var bindings = groups[i].AssignedBindings;
          var keys = groups[i].Keys;

          machineData.Write($"{groupTag}-name", groups[i].Name);

          machineData.Write($"{groupTag}-keys-count", keys.Count);
          for (int j = 0; j < keys.Count; j++)
          {
            var keyTag = $"{groupTag}-keys-{j}";
            machineData.Write($"{keyTag}", keys[j].ToString());
          }

          machineData.Write($"{groupTag}-bindings-count", bindings.Count);
          for (int j = 0; j < bindings.Count; j++)
          {
            var bindingTag = $"{groupTag}-bindings-{j}";
            machineData.Write($"{bindingTag}-guid", bindings[j].Guid.ToString());
            machineData.Write($"{bindingTag}-keyIndex", bindings[j].KeyIndex);
          }
        }
      };

      XmlLoader.OnLoad += info =>
      {
        groups.Clear();
        var machineData = info.MachineData;

        if (!machineData.HasKey("keymanager-group-count"))
          return;

        var count = machineData.ReadInt("keymanager-group-count");
        groups.Capacity = count;

        for (int i = 0; i < count; i++)
        {
          var groupTag = $"keymanager-group-{i}";
          var name = machineData.ReadString($"{groupTag}-name");
          var keys = new List<KeyCode>();

          // Support old data saved without multikeybind support
          if (machineData.HasKey($"{groupTag}-keys-count"))
          {
            var keyCount = machineData.ReadInt($"{groupTag}-keys-count");
            for (int j = 0; j < keyCount; j++)
            {
              var keyTag = $"{groupTag}-keys-{j}";
              keys.Add((KeyCode) Enum.Parse(typeof(KeyCode), machineData.ReadString(keyTag)));
            }
          } else
          {
            keys.Add((KeyCode)Enum.Parse(typeof(KeyCode), machineData.ReadString($"{groupTag}-key")));
          }

          var group = new KeyGroup(name, keys);

          var bindingsCount = machineData.ReadInt($"{groupTag}-bindings-count");
          for (int j = 0; j < bindingsCount; j++)
          {
            var bindingTag = $"{groupTag}-bindings-{j}";
            var binding = new Keybinding(
              new Guid(machineData.ReadString($"{bindingTag}-guid")),
              machineData.ReadInt($"{bindingTag}-keyIndex"));
            group.AddKeybinding(binding);
          }
          groups.Add(group);
        }

        // Show interface when loading a machine with key manager data.
        KeyManagerInterface.Instance.SetActive();
      };

      // When a block is deleted, delete any corresponding keybindings
      Game.OnBlockRemoved += () =>
      {
        foreach (var group in groups)
        {
          foreach (var binding in new List<Keybinding>(group.AssignedBindings.Where(b => b.Block == null)))
          {
            group.RemoveKeybinding(binding);
          }
        }
        // If any groups were completely emtpied by the above checks, delete them entirely
        groups = new List<KeyGroup>(groups.Where(g => g.AssignedBindings.Count > 0));
      };
    }

    public void ResetKeyGroups()
    {
      // Called by MachineObjectTracker patch when a new machine is created
      groups.Clear();
    }

    public void CreateKeyGroup(string name, KeyCode key)
    {
      groups.Add(new KeyGroup(name, key));
    }

    public void DeleteKeyGroup(KeyGroup group)
    {
      groups.Remove(group);
    }

    private struct AutoAddGroupId
    {
      public Type type;
      public int keyIndex;
      public List<KeyCode> keys;

      public override bool Equals(object obj)
      {
        if (!(obj is AutoAddGroupId))
          return false;
        var other = (AutoAddGroupId) obj;
        return other.type == type && other.keyIndex == keyIndex && other.keys.SequenceEqual(keys);
      }
    }

    public void AutoAddGroups()
    {
      var groups = new Dictionary<AutoAddGroupId, KeyGroup>();

      foreach (var block in Machine.Active().BuildingBlocks)
      {
        if (block.Keys.Count == 0) continue;

        for (int i = 0; i < block.Keys.Count; i++)
        {
          var id = new AutoAddGroupId()
          {
            type = block.GetType(),
            keyIndex = i,
            keys = new List<KeyCode>(block.Keys[i].KeyCode)
          };

          if (!groups.ContainsKey(id))
          {
            groups.Add(id, new KeyGroup($"{block.name} {block.Keys[i].DisplayName}", block.Keys[i].KeyCode));
          }
          groups[id].AddKeybinding(block, i);
        }
      }

      foreach (var group in groups.Values)
      {
        this.groups.Add(group);
      }
    }
  }
}