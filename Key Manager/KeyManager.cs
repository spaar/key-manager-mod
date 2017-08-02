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

        const string keyManagerTag = "keymanager-v2";
        machineData.Write($"{keyManagerTag}-groups-count", groups.Count);
        for (int i = 0; i < groups.Count; i++)
        {
          var groupTag = $"{keyManagerTag}-groups-{i}";
          var bindings = groups[i].Keybindings.ToList();

          machineData.Write($"{groupTag}-name", groups[i].Name);

          machineData.Write($"{groupTag}-keys-count", bindings.Count);
          for (int j = 0; j < bindings.Count; j++)
          {
            var keyTag = $"{groupTag}-keys-{j}";
            machineData.Write($"{keyTag}-key", bindings[j].Key.ToString());

            machineData.Write($"{keyTag}-bindings-count", bindings[j].Value.Count);
            for (int k = 0; k < bindings[j].Value.Count; k++)
            {
              var bindingTag = $"{keyTag}-bindings-{k}";
              machineData.Write($"{bindingTag}-guid", bindings[j].Value[k].Guid.ToString());
              machineData.Write($"{bindingTag}-keyIndex", bindings[j].Value[k].MKeyIndex);
              machineData.Write($"{bindingTag}-keyCodeIndex", bindings[j].Value[k].KeyCodeIndex);
            }

          }
        }
      };

      XmlLoader.OnLoad += info =>
      {
        groups.Clear();
        var machineData = info.MachineData;

        const string keyManagerTag = "keymanager-v2";

        if (!machineData.HasKey($"{keyManagerTag}-groups-count"))
          return;

        var count = machineData.ReadInt($"{keyManagerTag}-groups-count");
        groups.Capacity = count;

        for (int i = 0; i < count; i++)
        {
          var groupTag = $"{keyManagerTag}-groups-{i}";
          var name = machineData.ReadString($"{groupTag}-name");

          var group = new KeyGroup(name);

          var keyCount = machineData.ReadInt($"{groupTag}-keys-count");
          for (int j = 0; j < keyCount; j++)
          {
            var keyTag = $"{groupTag}-keys-{j}";
            var key = Util.ParseEnum<KeyCode>(machineData.ReadString($"{keyTag}-key"));

            var bindingsCount = machineData.ReadInt($"{keyTag}-bindings-count");
            for (int k = 0; k < bindingsCount; k++)
            {
              var bindingTag = $"{keyTag}-bindings-{k}";

              var guid = new Guid(machineData.ReadString($"{bindingTag}-guid"));
              var keyIndex = machineData.ReadInt($"{bindingTag}-keyIndex");
              var keyCodeIndex = machineData.ReadInt($"{bindingTag}-keyCodeIndex");

              var binding = new Keybinding(guid, keyIndex, keyCodeIndex);
              group.AddKeybinding(key, binding);
            }
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
          group.RemoveAllBindingsWithoutBlock();
        }
        // If any groups were completely emtpied by the above checks, delete them entirely
        groups.RemoveAll(g => g.HasNoBindings());
      };
    }

    public void ResetKeyGroups()
    {
      // Called by MachineObjectTracker patch when a new machine is created
      groups.Clear();
    }

    public void CreateKeyGroup(string name)
    {
      groups.Add(new KeyGroup(name));
    }

    public void DeleteKeyGroup(KeyGroup group)
    {
      groups.Remove(group);
    }

    public void MoveUp(int index)
    {
      if (index == 0) return;

      var group = groups[index];
      groups.Remove(group);
      groups.Insert(index - 1, group);
    }

    public void MoveDown(int index)
    {
      if (index == groups.Count - 1) return;

      var group = groups[index];
      groups.Remove(group);
      groups.Insert(index + 1, group);
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
        var other = (AutoAddGroupId)obj;
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
            groups.Add(id, new KeyGroup($"{block.name} {block.Keys[i].DisplayName}"));
          }
          for (int j = 0; j < id.keys.Count; j++)
          {
            groups[id].AddKeybinding(id.keys[j], new Keybinding(block, i, j));
          }
        }
      }

      foreach (var group in groups.Values)
      {
        this.groups.Add(group);
      }
    }
  }
}