//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using BitMagic.DiscUtils.Streams;

namespace BitMagic.DiscUtils.LogicalDiskManager;

internal abstract class DatabaseRecord
{
    public uint Counter;
    public uint DataLength;
    public uint Flags;

    public ulong Id;
    public uint Label;
    public string Name;
    public RecordType RecordType;
    public string Signature; // VBLK
    public uint Valid;

    public static DatabaseRecord ReadFrom(byte[] buffer, int offset) => ReadFrom(buffer.AsSpan(offset));

    public static DatabaseRecord ReadFrom(ReadOnlySpan<byte> buffer)
    {
        DatabaseRecord result = null;

        if (EndianUtilities.ToInt32BigEndian(buffer.Slice(0xC)) != 0)
        {
            result = (RecordType)(buffer[0x13] & 0xF) switch
            {
                RecordType.Volume => new VolumeRecord(),
                RecordType.Component => new ComponentRecord(),
                RecordType.Extent => new ExtentRecord(),
                RecordType.Disk => new DiskRecord(),
                RecordType.DiskGroup => new DiskGroupRecord(),
                _ => throw new NotImplementedException($"Unrecognized record type: {buffer[0x13]}"),
            };
            result.DoReadFrom(buffer);
        }

        return result;
    }

    protected static ulong ReadVarULong(byte[] buffer, ref int offset)
    {
        int length = buffer[offset];

        ulong result = 0;
        for (var i = 0; i < length; ++i)
        {
            result = (result << 8) | buffer[offset + i + 1];
        }

        offset += length + 1;

        return result;
    }

    protected static long ReadVarLong(byte[] buffer, ref int offset)
    {
        return (long)ReadVarULong(buffer, ref offset);
    }

    protected static string ReadVarString(byte[] buffer, ref int offset)
    {
        int length = buffer[offset];

        var result = EndianUtilities.BytesToString(buffer, offset + 1, length);
        offset += length + 1;
        return result;
    }

    protected static byte ReadByte(byte[] buffer, ref int offset)
    {
        return buffer[offset++];
    }

    protected static uint ReadUInt(byte[] buffer, ref int offset)
    {
        offset += 4;
        return EndianUtilities.ToUInt32BigEndian(buffer, offset - 4);
    }

    protected static long ReadLong(byte[] buffer, ref int offset)
    {
        offset += 8;
        return EndianUtilities.ToInt64BigEndian(buffer, offset - 8);
    }

    protected static ulong ReadULong(byte[] buffer, ref int offset)
    {
        offset += 8;
        return EndianUtilities.ToUInt64BigEndian(buffer, offset - 8);
    }

    protected static string ReadString(byte[] buffer, int len, ref int offset)
    {
        offset += len;
        return EndianUtilities.BytesToString(buffer, offset - len, len);
    }

    protected static Guid ReadBinaryGuid(byte[] buffer, ref int offset)
    {
        offset += 16;
        return EndianUtilities.ToGuidBigEndian(buffer, offset - 16);
    }

    protected virtual void DoReadFrom(byte[] buffer, int offset) => DoReadFrom(buffer.AsSpan(offset));

    protected virtual void DoReadFrom(ReadOnlySpan<byte> buffer)
    {
        Signature = EndianUtilities.BytesToString(buffer.Slice(0x00, 4));
        Label = EndianUtilities.ToUInt32BigEndian(buffer.Slice(0x04));
        Counter = EndianUtilities.ToUInt32BigEndian(buffer.Slice(0x08));
        Valid = EndianUtilities.ToUInt32BigEndian(buffer.Slice(0x0C));
        Flags = EndianUtilities.ToUInt32BigEndian(buffer.Slice(0x10));
        RecordType = (RecordType)(Flags & 0xF);
        DataLength = EndianUtilities.ToUInt32BigEndian(buffer.Slice(0x14));
    }
}