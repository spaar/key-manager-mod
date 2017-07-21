using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyGroupEditInterface
  {
    public readonly int WindowID = Util.GetWindowID();
    private Rect windowRect = new Rect(1075, 450, 350, 200);

    private KeyGroup group;

    private bool assigningBlocks = false;
    private BlockBehaviour blockToAssign = null;
    private int selectedKeybind = -1;

    public void Show(KeyGroup group)
    {
      this.group = group;
      windowRect = GUI.Window(WindowID, windowRect, DoWindow, group.Name);
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
              group.RemoveKeybinding(block);
            }
            else
            {
              // Blocks without keybinds can't be selected
              if (block.Keys.Count > 0)
              {
                // Select block, open keybind selection window
                block.VisualController.SetHighlighted();
                blockToAssign = block;
              }
            }
          }
        }
      }

      if (assigningBlocks)
      {
        // Highlight all assigned blocks
        // Need to do this every frame, as AddPiece is gonna try to de-highlight them after mouse-over
        foreach (var binding in group.AssignedBindings)
        {
          binding.Block.VisualController.SetHighlighted();
        }
      }

      // If a keybind was selected, add it and go back to block selection mode
      if (blockToAssign != null && selectedKeybind != -1)
      {
        group.AddKeybinding(blockToAssign, selectedKeybind);
        selectedKeybind = -1;
        blockToAssign = null;
      }
    }

    private void DoWindow(int id)
    {
      if (blockToAssign != null)
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
      else if (assigningBlocks)
      {
        GUILayout.Label("Click non-highlighted blocks to add them to the key group, click highlighted blocks to remove them from the group.");
        if (GUILayout.Button("Exit assignment mode"))
        {
          ExitBlockAssignmentMode();
        }
      }
      else
      {
        if (GUILayout.Button("Close"))
        {
          KeyManagerInterface.Instance.CloseGroupEdit();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name: ");
        group.Name = GUILayout.TextField(group.Name);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("(Un-)Assign blocks"))
        {
          EnterBlockAssignmentMode();
        }
      }
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

      foreach (var binding in group.AssignedBindings)
      {
        binding.Block.VisualController.SetNormal();
      }
    }
  }
}