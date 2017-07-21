using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyManagerInterface : ModLoader.SingleInstance<KeyManagerInterface>
  {
    public override string Name { get; } = "Key Manager";

    public KeyManager KeyManager { get; set; }

    public readonly int WindowID = Util.GetWindowID();
    private Rect windowRect = new Rect(1000, 300, 500, 500);
    private bool editMode = false;
    private KeyGroup modifiyingGroup = null;
    private KeyGroupEditInterface editInterface = new KeyGroupEditInterface();

    public void Start()
    {
      DontDestroyOnLoad(this);
    }

    public void Update()
    {
      if (Game.AddPiece == null || StatMaster.isSimulating)
      {
        return;
      }

      if (modifiyingGroup != null)
      {
        editInterface.BuildingUpdate();
      }
    }

    public void OnGUI()
    {
      if (Game.AddPiece == null || StatMaster.isSimulating)
      {
        return;
      }

      GUI.skin = ModGUI.Skin;

      windowRect = GUI.Window(WindowID, windowRect, DoWindow, "Key Manager");

      if (modifiyingGroup != null)
      {
        editInterface.Show(modifiyingGroup);
      }
    }

    public void CloseGroupEdit()
    {
      modifiyingGroup = null;
    }

    private void DoWindow(int id)
    {
      if (GUILayout.Button("Edit"))
      {
        editMode = !editMode;
        if (!editMode) modifiyingGroup = null;
      }

      var groups = KeyManager.KeyGroups;

      GUILayout.BeginVertical();
      if (groups.Count > 0)
      {
        foreach (var group in groups)
        {
          DisplayGroup(group);
        }
      }
      else
      {
        GUILayout.Label("No key groups present.");
      }
      GUILayout.EndVertical();

      if (editMode)
      {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
        {
          KeyManager.CreateKeyGroup("New key-group", KeyCode.None);
        }

        GUILayout.Button("Auto-Add");

        GUILayout.EndHorizontal();
      }

      GUI.DragWindow();
    }

    private void DisplayGroup(KeyGroup group)
    {
      GUILayout.BeginHorizontal();
      if (editMode)
      {
        if (GUILayout.Button(group.Name))
        {
          modifiyingGroup = group;
        }

        if (GUILayout.Button("X"))
        {
          if (modifiyingGroup == group) modifiyingGroup = null;
          KeyManager.DeleteKeyGroup(group);
        }
      }
      else
      {
        GUILayout.Label(group.Name);
        GUILayout.Button(group.Key.ToString());
      }
      GUILayout.EndHorizontal();
    }
  }
}