using System;
using System.Linq;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyGroupEditInterface
  {
    public readonly int WindowID = ModLoader.Util.GetWindowID();
    private Rect windowRect;

    private KeyGroup group;

    // Reference to last edited group after pressing Tab, used to detect that we're now displaying the new group.
    // null after the first frame where the new group was displayed.
    private KeyGroup oldGroup;

    // Assigning blocks manually is not enabled atm, pending a good way to do it with multikeybind support.
    // This only serves to remove blocks from groups instead.
    private bool assigningBlocks = false;
    private BlockBehaviour blockToAssign = null;
    private int selectedKeybind = -1;

    public void Show(KeyGroup group)
    {
      this.group = group;
      windowRect = GUI.Window(WindowID, windowRect, DoWindow, group.Name);
    }

    public void LoadWindowPosition()
    {
      windowRect.x = Configuration.GetFloat("groupedit-x", 1185);
      windowRect.y = Configuration.GetFloat("groupedit-y", 460);
      windowRect.width = 370;
      windowRect.height = 180;
    }

    public void SaveWindowPosition()
    {
      Configuration.SetFloat("groupedit-x", windowRect.x);
      Configuration.SetFloat("groupedit-y", windowRect.y);
    }

    public void BuildingUpdate()
    {
      if (assigningBlocks && Input.GetButtonDown("Fire1"))
      {
        // Raycasting code basically stolen from AddPiece
        var mousePosition = (Vector2)Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0.0f));
        var hit = new RaycastHit();
        var rayHit = Physics.Raycast(ray, out hit, AddPiece.Instance.layerMasky);

        if (rayHit)
        {
          var block = hit.collider.gameObject.GetComponentInParent<BlockBehaviour>();
          if (block != null)
          {
            // User clicked on `block`

            if (group.HasBlock(block))
            {
              // Deselect block
              block.VisualController.SetNormal();
              group.RemoveAllBindingsWithBlock(block);
            }
            /*else
            {
              // Blocks without keybinds can't be selected
              if (block.Keys.Count > 0)
              {
                // Select block, open keybind selection window
                block.VisualController.SetHighlighted();
                blockToAssign = block;
              }
            }*/
          }
        }
      }

      if (assigningBlocks)
      {
        // Highlight all assigned blocks
        // Need to do this every frame, as AddPiece is gonna try to de-highlight them after mouse-over
        foreach (var block in group.AllAssignedBlocks())
        {
          block.VisualController.SetHighlighted();
        }
      }

      // If a keybind was selected, add it and go back to block selection mode
      /*if (blockToAssign != null && selectedKeybind != -1)
      {
        group.AddKeybinding(blockToAssign, selectedKeybind);
        selectedKeybind = -1;
        blockToAssign = null;
      }*/
    }

    private void DoWindow(int id)
    {
      /*if (blockToAssign != null)
      {
        GUILayout.Label("Select which keybind to add:");
        GUILayout.BeginHorizontal();
        for (int i = 0; i < blockToAssign.Keys.Count; i++)
        {
          var key = blockToAssign.Keys[i];
          if (GUILayout.Button(key.DisplayName))
          {
            // BuildingUpdate() will handle adding the keybind
            selectedKeybind = i;
          }
        }
        GUILayout.EndHorizontal();
      }
      else */
      if (assigningBlocks)
      {
        /*GUILayout.Label(@"Click non-highlighted blocks to add them to the key group.
Click highlighted blocks to remove them from the group.");*/
        GUILayout.Label("Click a highlighted block to remove it from the group.");

        if (GUILayout.Button($"Assign all controls bound to {group.KeyString()}"))
        {
          group.AddAllWithKeys();
        }

        if (GUILayout.Button("Exit assignment mode"))
        {
          ExitBlockAssignmentMode();
        }
      }
      else
      {
        var closeRect = new Rect(windowRect.width - 40, 6, 32, 32);
        if (GUI.Button(closeRect, "X"))
        {
          KeyManagerInterface.Instance.CloseGroupEdit();
        }

        GUILayout.BeginHorizontal();

        var textStyle = new GUIStyle(Elements.Labels.Title)
        {
          margin = { top = 12 },
          fontSize = 15
        };
        GUILayout.Label("Name: ", textStyle);

        GUI.SetNextControlName("txt-group-name");
        group.Name = GUILayout.TextField(group.Name);

        // Select all text after cycling through groups with tab
        if (oldGroup != null && oldGroup != group)
        {
          var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
          te.SelectAll();

          oldGroup = null;
        }

        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("(Un-)Assign blocks"))
        {
          EnterBlockAssignmentMode();
        }
      }

      // Cycle through groups with tab when editing the name
      if (GUI.GetNameOfFocusedControl() == "txt-group-name"
        && Event.current.type == EventType.KeyDown
        && Event.current.keyCode == KeyCode.Tab)
      {
        KeyManagerInterface.Instance.EditNextGroup();

        // Select all text once we're displaying the new group
        oldGroup = group;
      }

      GUI.DragWindow();
    }

    private void EnterBlockAssignmentMode()
    {
      assigningBlocks = true;

      // Disable other tools
      var toolController = GameObject.Find("HUD/TopBar/Middle/TOOL CONTROLLER").GetComponent<MachineToolController>();
      toolController.DisableAll();

      // Don't want to place blocks when trying to select
      AddPiece.disableBlockPlacement = true;
      // Take control of highlighting
      AddPiece.disableBlockHighlight = true;

    }

    private void ExitBlockAssignmentMode()
    {
      assigningBlocks = false;

      AddPiece.disableBlockPlacement = false;
      AddPiece.disableBlockHighlight = false;

      foreach (var block in group.AllAssignedBlocks())
      {
        block.VisualController.SetNormal();
      }
    }
  }
}