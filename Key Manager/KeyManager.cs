using System;
using System.Collections.Generic;
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
          machineData.Write($"{groupTag}-name", groups[i].Name);
          machineData.Write($"{groupTag}-key", groups[i].Key.ToString());
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
          Debug.Log($"Group {i}");
          var groupTag = $"keymanager-group-{i}";
          var group = new KeyGroup(
            machineData.ReadString($"{groupTag}-name"),
            (KeyCode)Enum.Parse(typeof(KeyCode), machineData.ReadString($"{groupTag}-key")));
          var bindingsCount = machineData.ReadInt($"{groupTag}-bindings-count");
          for (int j = 0; j < bindingsCount; j++)
          {
            Debug.Log($"Binding {j}");
            var bindingTag = $"{groupTag}-bindings-{j}";
            var binding = new Keybinding(
              new Guid(machineData.ReadString($"{bindingTag}-guid")),
              machineData.ReadInt($"{bindingTag}-keyIndex"));
            group.AddKeybinding(binding);
          }
          groups.Add(group);
        }

        KeyManagerInterface.Instance.SetActive();
      };

      // TODO: Clear groups when deleting machine
    }

    public void CreateKeyGroup(string name, KeyCode key)
    {
      groups.Add(new KeyGroup(name, key));
    }

    public void DeleteKeyGroup(KeyGroup group)
    {
      groups.Remove(group);
    }
  }
}