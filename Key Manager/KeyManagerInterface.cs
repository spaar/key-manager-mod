﻿using System;
using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader;
using spaar.ModLoader.Internal.Tools;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyManagerInterface : ModLoader.SingleInstance<KeyManagerInterface>
  {
    public override string Name { get; } = "Key Manager";

    public KeyManager KeyManager { get; set; }

    public readonly int WindowID = ModLoader.Util.GetWindowID();
    private Rect windowRect;
    private Vector2 scrollPosition = new Vector2(0f, 0f);

    private Key toggleKey;
    private SettingsButton button;
    private bool active = false;

    private bool editMode = false;
    private KeyGroup modifiyingGroup = null;
    private KeyGroupEditInterface editInterface = new KeyGroupEditInterface();

    private bool showExampleDialog = false;
    private ExampleMachineDialog exampleDialog = new ExampleMachineDialog();

    private GUIStyle IconButtonStyle;

    // The mechanics of the "hover-to-remap" keymap technique are mostly taken from the mod loader's Keymapper
    private KeyGroup currentGroupToMap = null;
    private int currentKeyToMap = -1;
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
      KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
      KeyCode.Period, KeyCode.Comma, KeyCode.Return,
      KeyCode.KeypadPeriod, KeyCode.KeypadEnter,
      KeyCode.KeypadDivide, KeyCode.KeypadMultiply, KeyCode.KeypadMinus, KeyCode.KeypadPlus,
    };

    public void Start()
    {
      DontDestroyOnLoad(this);

      button = new SettingsButton()
      {
        Text = "Keys",
        Value = false,
        OnToggle = OnInterfaceToggle
      };
      button.Create();

      windowRect.x = Configuration.GetFloat("main-x", 1100f);
      windowRect.y = Configuration.GetFloat("main-y", 309);
      windowRect.width = 400;
      windowRect.height = 500;
      editInterface.LoadWindowPosition();

      toggleKey = Keybindings.AddKeybinding("Key Manager",
        new Key(KeyCode.RightControl, KeyCode.M));
    }

    public void OnUnload()
    {
      Configuration.SetFloat("main-x", windowRect.x);
      Configuration.SetFloat("main-y", windowRect.y);
      editInterface.SaveWindowPosition();
      Configuration.Save();
    }

    public void SetActive()
    {
      button.Value = true;

      // Don't show the example machine dialog if we just loaded a machine with key manager support.
      showExampleDialog = false;
    }

    private void OnInterfaceToggle(bool value)
    {
      active = value;

      // Show example machine dialog if not previously dismissed
      var dismissedExampleDialog = Configuration.GetBool("dismissed-example-dialog", false);
      if (!dismissedExampleDialog)
      {
        showExampleDialog = true;
      }
    }

    public void Update()
    {
      if (Game.AddPiece == null || StatMaster.isSimulating)
      {
        return;
      }

      if (toggleKey.Pressed())
      {
        button.Value = !button.Value;
      }

      if (!active)
      {
        return;
      }

      if (modifiyingGroup != null)
      {
        editInterface.BuildingUpdate();
      }
      else if (currentGroupToMap != null)
      {
        var keyToSet = KeyCode.None;

        if (Input.inputString.Length > 0 && !Input.inputString.Contains('\u0008' + ""))
        {
          var key = KeyCode.None;
          try
          {
            key = (KeyCode)Enum.Parse(typeof(KeyCode),
              (Input.inputString[0] + "").ToUpper());
          }
          catch (Exception e) { }

          keyToSet = key;
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
          keyToSet = keyCode;
        }

        if (keyToSet != KeyCode.None)
        {
          if (currentKeyToMap < currentGroupToMap.Keybindings.Keys.Count)
          {
            var previousKey = currentGroupToMap.Keybindings.Keys.ElementAt(currentKeyToMap);
            currentGroupToMap.ChangeKey(previousKey, keyToSet);
          }
          else
          {
            currentGroupToMap.AddKey(keyToSet);
          }
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

      if (showExampleDialog)
      {
        exampleDialog.Show();
      }
    }

    public void CloseExampleDialog(bool dismissed)
    {
      showExampleDialog = false;

      if (dismissed)
      {
        Configuration.SetBool("dismissed-example-dialog", true);
      }
    }

    public void CloseGroupEdit()
    {
      modifiyingGroup = null;
    }

    public void EditNextGroup()
    {
      if (modifiyingGroup == null) return;

      var groups = KeyManager.KeyGroups;
      var index = groups.IndexOf(modifiyingGroup);
      index++;
      if (index >= groups.Count)
      {
        index = 0;
      }

      modifiyingGroup = groups[index];
    }

    private void DoWindow(int id)
    {
      var editRect = new Rect(windowRect.width - 40, 6, 32, 32);

      if (GUI.Button(editRect, Resources.Edit, IconButtonStyle))
      {
        editMode = !editMode;
        if (!editMode) modifiyingGroup = null;
      }

      var groups = new List<KeyGroup>(KeyManager.KeyGroups);

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
          KeyManager.CreateKeyGroup("New key group");
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
          currentKeyToMap = -1;
        }
        else
        {
          currentGroupToMap = groups[int.Parse(GUI.tooltip.Split('-')[0])];
          currentKeyToMap = int.Parse(GUI.tooltip.Split('-')[1]);
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

        if (GUILayout.Button("↑", index == 0
          ? Elements.Buttons.Disabled
          : Elements.Buttons.Default, GUILayout.Width(28f)))
        {
          KeyManager.MoveUp(index);
        }

        if (GUILayout.Button("↓", index == KeyManager.KeyGroups.Count - 1
          ? Elements.Buttons.Disabled
          : Elements.Buttons.Default, GUILayout.Width(28f)))
        {
          KeyManager.MoveDown(index);
        }

        if (GUILayout.Button(Resources.Delete, IconButtonStyle, GUILayout.Height(28f), GUILayout.Width(28f)))
        {
          if (modifiyingGroup == group) modifiyingGroup = null;
          KeyManager.DeleteKeyGroup(group);
        }
      }
      else
      {
        var keys = group.Keybindings.Keys.ToList();
        var i = 0;
        for (; i < keys.Count; i++)
        {
          GUILayout.Button(new GUIContent(keys[i].ToString(), $"{tooltip}-{i}"),
            Elements.Buttons.Red, GUILayout.Width(110f));
        }
        GUILayout.Button(new GUIContent("Add new", $"{tooltip}-{i}"),
          Elements.Buttons.Red, GUILayout.Width(110f));
      }
      GUILayout.EndHorizontal();
    }
  }
}