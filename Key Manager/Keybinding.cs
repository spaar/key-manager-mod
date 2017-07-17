using System;

namespace spaar.Mods.KeyManager
{
  public class Keybinding
  {
    public MKey Key { get; private set; }
    public int KeyIndex { get; private set; }
    public Guid Guid { get; private set; }

    public BlockBehaviour Block
    {
      get { return Machine.Active().BuildingBlocks.Find(b => b.Guid == Guid); }
    }

    public Keybinding(BlockBehaviour block, int keyIndex)
    {
      Guid = block.Guid;
      KeyIndex = keyIndex;

      if (Block == null) throw new ArgumentException($"No such block: {block.Guid}", "block");

      Key = block.Keys[keyIndex];
    }
  }
}