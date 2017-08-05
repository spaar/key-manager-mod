using spaar.ModLoader.UI;
using Steamworks;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public class ExampleMachineDialog
  {
    private int WindowIDOpenModal = ModLoader.Util.GetWindowID();
    private Rect windowRectOpenModal = new Rect(500, 200, 300, 200);

    private bool showMainWindow = false;
    private int WindowIDMain = ModLoader.Util.GetWindowID();
    private Rect windowRectMain = new Rect(500, 200, 400, 450);

    private GUIStyle textStyle;

    public void Show()
    {
      textStyle = new GUIStyle(Elements.Labels.Title)
      {
        fontSize = 14
      };

      if (showMainWindow)
      {
        windowRectMain = GUILayout.Window(WindowIDMain, windowRectMain, DoWindowMain, "Lockheed Martin F-22 Raptor");
      }
      else
      {
        windowRectOpenModal = GUILayout.Window(WindowIDOpenModal, windowRectOpenModal, DoWindowOpenModal,
          "Example machine");
      }
    }


    private void DoWindowOpenModal(int id)
    {
      GUILayout.Label("New to the Key Manager?\nCheck out an example machine to see what it can do!");

      if (GUILayout.Button("Show me more"))
      {
        showMainWindow = true;
      }

      if (GUILayout.Button("No, thanks"))
      {
        Close(false);
      }

      if (GUILayout.Button("Don't show this again"))
      {
        Close(true);
      }

      GUI.DragWindow();
    }

    private void DoWindowMain(int id)
    {
      var closeRect = new Rect(windowRectMain.width - 40, 6, 32, 32);
      if (GUI.Button(closeRect, "X"))
      {
        Close(false);
      }

      GUILayout.Label(Resources.F22Thumbnail);

      GUILayout.Space(5f);

      GUILayout.Label(@"Redstoneman's F-22 Raptor is the first machine to include Key Manager
support. It demonstrates how the Key Manager can make a machine
more user-friendly.", textStyle);

      if (GUILayout.Button("Load it up!"))
      {
        Resources.LoadF22();
        Close(false);
      }

      GUILayout.Label("Make sure to save any unsaved progress before loading!");

      if (GUILayout.Button("Show me the workshop page!"))
      {
        SteamFriends.ActivateGameOverlayToWebPage("http://steamcommunity.com/sharedfiles/filedetails/?id=936738075");
      }

      GUI.DragWindow();
    }

    private void Close(bool dontShowAgain)
    {
      KeyManagerInterface.Instance.CloseExampleDialog(dontShowAgain);

      showMainWindow = false;
    }
  }
}