using System;

namespace spaar.Mods.KeyManager
{
  public class Keybinding
  {
    public int KeyIndex { get; private set; }
    public Guid Guid { get; private set; }

    public MKey Key
    {
      get { return Block.Keys[KeyIndex]; }
    }

    public BlockBehaviour Block
    {
      get { return Machine.Active().BuildingBlocks.Find(b => b.Guid == Guid); }
    }

    public Keybinding(BlockBehaviour block, int keyIndex)
    {
      Guid = block.Guid;
      KeyIndex = keyIndex;
    }

    public Keybinding(Guid guid, int keyIndex)
    {
      Guid = guid;
      KeyIndex = keyIndex;
    }
  }
}