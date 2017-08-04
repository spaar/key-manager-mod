using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace spaar.Mods.KeyManager
{
  public static class Textures
  {
    public static Texture2D Edit { get; private set; }
    public static Texture2D Delete { get; private set; }

    public static void Init()
    {
      Edit = LoadTexture("edit");
      Delete = LoadTexture("delete");
    }

    /*private static Texture2D LoadTexture(string name)
    {
      var path = Application.dataPath + "/Mods/Resources/KeyManager/" + name + ".png";

      try
      {
        var bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);
        return texture;
      }
      catch (Exception e)
      {
        Debug.LogError($"Error loading texture: {name}");
        Debug.LogException(e);
      }
      return null;
    }*/

    private static Texture2D LoadTexture(string name)
    {
      try
      {
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream($"spaar.Mods.KeyManager.Resources.{name}.png"))
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