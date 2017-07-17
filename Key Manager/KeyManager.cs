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