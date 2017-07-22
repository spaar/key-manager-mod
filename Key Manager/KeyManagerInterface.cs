using System;
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
    private Rect windowRect;
    private Vector2 scrollPosition = new Vector2(0f, 0f);

    private SettingsButton button;
    private bool active = false;

    private bool editMode = false;
    private KeyGroup modifiyingGroup = null;
    private KeyGroupEditInterface editInterface = new KeyGroupEditInterface();

    private GUIStyle IconButtonStyle;

    // The mechanics of the "hover-to-remap" keymap technique are mostly taken from the mod loader's Keymapper
    private KeyGroup currentGroupToMap = null;
    private static KeyCode[] SpecialKeys =
    {
      KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftShift,
      KeyCode.RightShift, KeyCode.LeftAlt, KeyCode.RightAlt,
      KeyCode.Backspace, KeyCode.Mouse0, KeyCode.Mouse1,
      KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4,
      KeyCode.Mouse5, KeyCode.Mouse6, KeyCode.Alpha0,
      KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
      KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
      KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
      KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4,
      KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8,
      KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12,
      KeyCode.F13, KeyCode.F14, KeyCode.F15, KeyCode.Keypad0,
      KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3,
      KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6,
      KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9,
      KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow
    };

    public void SetActive()
    {
      button.Value = true;
    }

    public void Start()
    {
      DontDestroyOnLoad(this);

      button = new SettingsButton()
      {
        Text = "Keys",
        Value = false,
        OnToggle = val => active = val
      };
      button.Create();

      windowRect.x = Configuration.GetFloat("main-x", 1100f);
      windowRect.y = Configuration.GetFloat("main-y", 309);
      windowRect.width = 350;
      windowRect.height = 500;
      editInterface.LoadWindowPosition();
    }

    public void OnUnload()
    {
      Configuration.SetFloat("main-x", windowRect.x);
      Configuration.SetFloat("main-y", windowRect.y);
      editInterface.SaveWindowPosition();
      Configuration.Save();
    }

    public void Update()
    {
      if (Game.AddPiece == null || StatMaster.isSimulating || !active)
      {
        return;
      }

      if (modifiyingGroup != null)
      {
        editInterface.BuildingUpdate();
      }
      else if (currentGroupToMap != null)
      {
        if (Input.inputString.Length > 0 && !Input.inputString.Contains('\u0008' + ""))
        {
          currentGroupToMap.SetKey((KeyCode)Enum.Parse(typeof(KeyCode),
            (Input.inputString[0] + "").ToUpper()));
        }

        var keyCode = KeyCode.None;
        foreach (var key in SpecialKeys)
        {
          if (Input.GetKeyDown(key))
          {
            keyCode = key;
            break;
          }
        }
        if (keyCode != KeyCode.None)
        {
          currentGroupToMap.SetKey(keyCode);
        }
      }
    }

    public void OnGUI()
    {
      if (Game.AddPiece == null || StatMaster.isSimulating || !active)
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
        for (int i = 0; i < groups.Count; i++)
        {
          DisplayGroup(i);
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

        if (GUILayout.Button("Auto-Add"))
        {
          KeyManager.AutoAddGroups();
        }

        GUILayout.EndHorizontal();
      }

      // Check if a keybind button is hovered over
      if (Event.current.type == EventType.Repaint)
      {
        if (GUI.tooltip == "")
        {
          currentGroupToMap = null;
        }
        else
        {
          currentGroupToMap = groups[int.Parse(GUI.tooltip)];
        }
      }

      GUI.DragWindow();
    }

    private void DisplayGroup(int index)
    {
      var group = KeyManager.KeyGroups[index];
      var tooltip = "" + index;

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
          GUI.BringWindowToBack(WindowID);
        }

        if (GUILayout.Button(Textures.Delete, IconButtonStyle, GUILayout.Height(28f), GUILayout.Width(28f)))
        {
          if (modifiyingGroup == group) modifiyingGroup = null;
          KeyManager.DeleteKeyGroup(group);
        }
      }
      else
      {
        GUILayout.Button(new GUIContent(group.Key.ToString(), tooltip),
          Elements.Buttons.Red, GUILayout.Width(110f));
      }
      GUILayout.EndHorizontal();
    }
  }
}