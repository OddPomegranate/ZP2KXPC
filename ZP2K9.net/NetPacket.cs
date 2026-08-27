using System.IO;

namespace ZP2K9.net;

// PC replacement for Microsoft.Xna.Framework.Net.PacketWriter / PacketReader.
// MonoGame doesn't ship Microsoft.Xna.Framework.Net at all, so these two types
// (plus SendDataOptions below) exist purely so every gameplay file that used
// them as plain read/write buffers - characters, particles, weapons, kills -
// keeps compiling with a ONE-LINE change: swap
//   using Microsoft.Xna.Framework.Net;
// for
//   using ZP2K9.net;
// No other code in those files needs to change; the member surface
// (Write/Read* via BinaryWriter/BinaryReader, plus Length/Position) is the
// same shape the original XNA types exposed.
//
// These are transport-agnostic on purpose: whatever INetworkSession backend
// ends up sending the bytes (LAN test harness, then Steamworks), it just
// needs the raw byte[] out of a PacketWriter and to feed bytes into a
// PacketReader - see PacketWriter.ToArray()/PacketReader.LoadFrom() below.

public class PacketWriter : BinaryWriter
{
    public PacketWriter()
        : base(new MemoryStream())
    {
    }

    public int Length => (int)BaseStream.Length;

    public new int Position
    {
        get => (int)BaseStream.Position;
        set => BaseStream.Position = value;
    }

    // Truncates back to empty and rewinds to the start, ready for the next
    // packet. Call this before building each new outgoing packet - the
    // original XNA PacketWriter did NOT do this automatically on SendData,
    // and neither does this one; see NetPlay.cs for where it's called.
    public void Reset()
    {
        BaseStream.SetLength(0);
        BaseStream.Position = 0;
    }

    public byte[] ToArray()
    {
        return ((MemoryStream)BaseStream).ToArray();
    }
}

public class PacketReader : BinaryReader
{
    public PacketReader()
        : base(new MemoryStream())
    {
    }

    public int Length => (int)BaseStream.Length;

    public new int Position
    {
        get => (int)BaseStream.Position;
        set => BaseStream.Position = value;
    }

    // Points this reader at a freshly-received packet's bytes.
    public void LoadFrom(byte[] data, int offset, int count)
    {
        MemoryStream stream = (MemoryStream)BaseStream;
        stream.SetLength(0);
        stream.Write(data, offset, count);
        stream.Position = 0;
    }
}

// Matches the real XNA enum's numeric values (it was a [Flags] enum) since
// NetPlay.cs casts raw ints/bools into this type - e.g.
// (SendDataOptions)(flag4 ? 1 : 2).
[System.Flags]
public enum SendDataOptions
{
    None = 0,
    Reliable = 1,
    InOrder = 2,
    Chat = 4
}
