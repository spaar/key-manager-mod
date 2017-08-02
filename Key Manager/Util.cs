using System;

namespace spaar.Mods.KeyManager
{
  public static class Util
  {
    public static T ParseEnum<T>(string str)
    {
      return (T) Enum.Parse(typeof(T), str);
    }
  }
}