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
using System.Collections.Generic;
using System.Text;
using BitMagic.DiscUtils.Streams;
using BitMagic.DiscUtils.Streams.Compatibility;

namespace BitMagic.DiscUtils.Partitions;

internal class GptEntry : IComparable<GptEntry>
{
    public ulong Attributes;
    public long FirstUsedLogicalBlock;
    public Guid Identity;
    public long LastUsedLogicalBlock;
    public string Name;
    public Guid PartitionType;

    public GptEntry()
    {
        PartitionType = Guid.Empty;
        Identity = Guid.Empty;
        Name = string.Empty;
    }

    public string FriendlyPartitionType => GetFriendlyPartitionType(PartitionType);

    private readonly static Dictionary<Guid, string> _friendlyPartitionTypeNames = new Dictionary<Guid, string>
    {
        { new Guid("00000000-0000-0000-0000-000000000000"), "Unused" },
        { new Guid("024DEE41-33E7-11D3-9D69-0008C781F39F"), "MBR Partition Scheme" },
        { new Guid("C12A7328-F81F-11D2-BA4B-00A0C93EC93B"), "EFI System" },
        { new Guid("21686148-6449-6E6F-744E-656564454649"), "BIOS Boot" },
        { new Guid("E3C9E316-0B5C-4DB8-817D-F92DF00215AE"), "Microsoft Reserved" },
        { new Guid("EBD0A0A2-B9E5-4433-87C0-68B6B72699C7"), "Windows Basic Data" },
        { new Guid("5808C8AA-7E8F-42E0-85D2-E1E90434CFB3"), "Windows Logical Disk Manager Metadata" },
        { new Guid("AF9B60A0-1431-4F62-BC68-3311714A69AD"), "Windows Logical Disk Manager Data" },
        { new Guid("75894C1E-3AEB-11D3-B7C1-7B03A0000000"), "HP-UX Data" },
        { new Guid("E2A1E728-32E3-11D6-A682-7B03A0000000"), "HP-UX Service" },
        { new Guid("A19D880F-05FC-4D3B-A006-743F0F84911E"), "Linux RAID" },
        { new Guid("0657FD6D-A4AB-43C4-84E5-0933C84B4F4F"), "Linux Swap" },
        { new Guid("E6D6D379-F507-44C2-A23C-238F2A3DF928"), "Linux Logical Volume Manager" },
        { new Guid("83BD6B9D-7F41-11DC-BE0B-001560B84F0F"), "FreeBSD Boot" },
        { new Guid("516E7CB4-6ECF-11D6-8FF8-00022D09712B"), "FreeBSD Data" },
        { new Guid("516E7CB5-6ECF-11D6-8FF8-00022D09712B"), "FreeBSD Swap" },
        { new Guid("516E7CB6-6ECF-11D6-8FF8-00022D09712B"), "FreeBSD Unix File System" },
        { new Guid("516E7CB8-6ECF-11D6-8FF8-00022D09712B"), "FreeBSD Vinum volume manager" },
        { new Guid("516E7CBA-6ECF-11D6-8FF8-00022D09712B"), "FreeBSD ZFS" },
        { new Guid("48465300-0000-11AA-AA11-00306543ECAC"), "Mac OS X HFS+" },
        { new Guid("55465300-0000-11AA-AA11-00306543ECAC"), "Mac OS X UFS" },
        { new Guid("6A898CC3-1DD2-11B2-99A6-080020736631"), "Mac OS X ZFS" },
        { new Guid("52414944-0000-11AA-AA11-00306543ECAC"), "Mac OS X RAID" },
        { new Guid("52414944-5F4F-11AA-AA11-00306543ECAC"), "Mac OS X RAID, Offline" },
        { new Guid("426F6F74-0000-11AA-AA11-00306543ECAC"), "Mac OS X Boot" },
        { new Guid("4C616265-6C00-11AA-AA11-00306543ECAC"), "Mac OS X Label" },
        { new Guid("49F48D32-B10E-11DC-B99B-0019D1879648"), "NetBSD Swap" },
        { new Guid("49F48D5A-B10E-11DC-B99B-0019D1879648"), "NetBSD Fast File System" },
        { new Guid("49F48D82-B10E-11DC-B99B-0019D1879648"), "NetBSD Log-Structed File System" },
        { new Guid("49F48DAA-B10E-11DC-B99B-0019D1879648"), "NetBSD RAID" },
        { new Guid("2DB519C4-B10F-11DC-B99B-0019D1879648"), "NetBSD Concatenated" },
        { new Guid("2DB519EC-B10F-11DC-B99B-0019D1879648"), "NetBSD Encrypted" }
    };

    public static string GetFriendlyPartitionType(Guid type) => _friendlyPartitionTypeNames.TryGetValue(type, out var name) ? name : type.ToString().ToUpperInvariant();

    public int CompareTo(GptEntry other)
    {
        return FirstUsedLogicalBlock.CompareTo(other.FirstUsedLogicalBlock);
    }

    public void ReadFrom(ReadOnlySpan<byte> buffer)
    {
        PartitionType = EndianUtilities.ToGuidLittleEndian(buffer);
        Identity = EndianUtilities.ToGuidLittleEndian(buffer.Slice(16));
        FirstUsedLogicalBlock = EndianUtilities.ToInt64LittleEndian(buffer.Slice(32));
        LastUsedLogicalBlock = EndianUtilities.ToInt64LittleEndian(buffer.Slice(40));
        Attributes = EndianUtilities.ToUInt64LittleEndian(buffer.Slice(48));
        Name = Encoding.Unicode.GetString(buffer.Slice(56, 72)).TrimEnd('\0');
    }

    public void WriteTo(Span<byte> buffer)
    {
        EndianUtilities.WriteBytesLittleEndian(PartitionType, buffer);
        EndianUtilities.WriteBytesLittleEndian(Identity, buffer.Slice(16));
        EndianUtilities.WriteBytesLittleEndian(FirstUsedLogicalBlock, buffer.Slice(32));
        EndianUtilities.WriteBytesLittleEndian(LastUsedLogicalBlock, buffer.Slice(40));
        EndianUtilities.WriteBytesLittleEndian(Attributes, buffer.Slice(48));
        var nameBytes = Encoding.Unicode.GetBytes(Name.AsSpan(), buffer.Slice(56));
        if (nameBytes < 36 * 2)
        {
            buffer.Slice(56 + nameBytes, 36 * 2 - nameBytes).Clear();
        }
    }
}