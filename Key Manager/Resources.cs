using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public static class Resources
  {
    public static Texture2D Edit { get; private set; }
    public static Texture2D Delete { get; private set; }
    public static Texture2D F22Thumbnail { get; private set; }

    private static readonly string F22Path = Application.dataPath + "/Mods/Resources/KeyManager/f22-raptor.bsg";

    public static void Init()
    {
      Edit = LoadTexture("edit");
      Delete = LoadTexture("delete");
      F22Thumbnail = LoadTexture("f22-raptor");
    }

    private static void ExtractF22()
    {
      if (File.Exists(F22Path)) return;

      Directory.CreateDirectory(Application.dataPath + "/Mods/Resources/KeyManager/");

      var fileStream = File.Create(F22Path);
      var resStream = GetResourceStream("f22-raptor.bsg");

      var buffer = new byte[4096];
      var bytesRead = 0;
      while ((bytesRead = resStream.Read(buffer, 0, buffer.Length)) > 0)
      {
        fileStream.Write(buffer, 0, bytesRead);
      }

      resStream.Close();
      fileStream.Close();
    }

    public static void LoadF22()
    {
      ExtractF22();
      var machineInfo = XmlLoader.LoadFullPath(F22Path);
      var loadWindow = GameObject.Find("HUD").transform.FindChild("LOAD WINDOW");
      var loadButton = loadWindow.FindChild("LOAD BUTTON").GetComponent<LoadButton>();
      loadWindow.gameObject.SetActive(true);
      loadButton.StartCoroutine(loadButton.LoadMachine(machineInfo));
      KeyManagerInterface.Instance.StartCoroutine(DeactivateAfterLoading(loadWindow.gameObject));
    }

    private static IEnumerator DeactivateAfterLoading(GameObject loadWindow)
    {
      yield return null;
      loadWindow.SetActive(false);
    }

    private static Texture2D LoadTexture(string name)
    {
      try
      {
        using (var stream = GetResourceStream($"{name}.png"))
        {
          var bytes = ReadAllBytes(stream);
          var texture = new Texture2D(0, 0);
          texture.LoadImage(bytes);
          return texture;
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"Error loading texture: {name}");
        Debug.LogException(e);
        return null;
      }
    }

    private static Stream GetResourceStream(string name)
    {
      var assembly = Assembly.GetExecutingAssembly();
      return assembly.GetManifestResourceStream($"spaar.Mods.KeyManager.Resources.{name}");
    }

    private static byte[] ReadAllBytes(Stream stream)
    {
      using (var memStream = new MemoryStream())
      {
        var buffer = new byte[512];
        var bytesRead = 0;
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
          memStream.Write(buffer, 0, bytesRead);
        }

        return memStream.ToArray();
      }
    }
  }
}