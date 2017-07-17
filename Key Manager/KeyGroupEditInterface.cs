using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class KeyGroupEditInterface
  {
    public readonly int WindowID = Util.GetWindowID();
    private Rect windowRect = new Rect(1075, 450, 350, 200);

    private KeyGroup group;

    public void Show(KeyGroup group)
    {
      this.group = group;
      windowRect = GUI.Window(WindowID, windowRect, DoWindow, group.Name);
    }

    private void DoWindow(int id)
    {
      if (GUILayout.Button("Close"))
      {
        KeyManagerInterface.Instance.CloseGroupEdit();
      }

      GUILayout.BeginHorizontal();
      GUILayout.Label("Name: ");
      group.Name = GUILayout.TextField(group.Name);
      GUILayout.EndHorizontal();

      GUILayout.Button("(Un-)Assign blocks");
    }
  }
}