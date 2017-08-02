using System;

namespace spaar.Mods.KeyManager
{
  public class Keybinding
  {
    public int MKeyIndex { get; private set; }
    public Guid Guid { get; private set; }

    public int KeyCodeIndex { get; private set; }

    public MKey Key => Block.Keys[MKeyIndex];

    public BlockBehaviour Block
    {
      get { return Machine.Active().BuildingBlocks.Find(b => b.Guid == Guid); }
    }

    public Keybinding(BlockBehaviour block, int mKeyIndex, int keyCodeIndex) : this(block.Guid, mKeyIndex, keyCodeIndex)
    {
    }

    public Keybinding(Guid guid, int mKeyIndex, int keyCodeIndex)
    {
      Guid = guid;
      MKeyIndex = mKeyIndex;
      KeyCodeIndex = keyCodeIndex;
    }
  }
}