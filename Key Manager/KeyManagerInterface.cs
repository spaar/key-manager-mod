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
    private Rect windowRect = new Rect(1100, 300, 350, 500);
    private Vector2 scrollPosition = new Vector2(0f, 0f);

    private bool editMode = false;
    private KeyGroup modifiyingGroup = null;
    private KeyGroupEditInterface editInterface = new KeyGroupEditInterface();

    private GUIStyle IconButtonStyle;

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

      IconButtonStyle = new GUIStyle(Elements.Buttons.Default)
      {
        padding = new RectOffset(4, 4, 4, 4)
      };

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
      var editRect = new Rect(windowRect.width - 40, 6, 32, 32);

      if (GUI.Button(editRect, Textures.Edit, IconButtonStyle))
      {
        editMode = !editMode;
        if (!editMode) modifiyingGroup = null;
      }

      var groups = KeyManager.KeyGroups;

      scrollPosition = GUILayout.BeginScrollView(scrollPosition);
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
      GUILayout.EndScrollView();

      if (editMode)
      {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
        {
          KeyManager.CreateKeyGroup("New key group", KeyCode.None);
        }

        GUILayout.Button("Auto-Add");

        GUILayout.EndHorizontal();
      }

      GUI.DragWindow();
    }

    private void DisplayGroup(KeyGroup group)
    {
      GUILayout.BeginHorizontal();

      var nameStyle = new GUIStyle(Elements.Labels.Title)
      {
        margin = { top = 12 },
        fontSize = 15
      };
      GUILayout.Label(group.Name, nameStyle);

      if (editMode)
      {
        if (GUILayout.Button("...", GUILayout.Width(40f)))
        {
          modifiyingGroup = group;
        }

        if (GUILayout.Button(Textures.Delete, IconButtonStyle, GUILayout.Height(28f), GUILayout.Width(28f)))
        {
          if (modifiyingGroup == group) modifiyingGroup = null;
          KeyManager.DeleteKeyGroup(group);
        }
      }
      else
      {
        GUILayout.Button(group.Key.ToString(), Elements.Buttons.Red, GUILayout.Width(110f));
      }
      GUILayout.EndHorizontal();
    }
  }
}