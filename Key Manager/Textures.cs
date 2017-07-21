using System;
using System.IO;
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

    private static Texture2D LoadTexture(string name)
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
    }
  }
}