﻿//
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
using DiscUtils.Streams;

namespace DiscUtils.HfsPlus;

internal abstract class CommonCatalogFileInfo : IByteArraySerializable
{
    public DateTime AccessTime;
    public DateTime AttributeModifyTime;
    public DateTime BackupTime;
    public DateTime ContentModifyTime;
    public DateTime CreateTime;
    public CatalogNodeId FileId;
    public UnixFileSystemInfo FileSystemInfo;
    public CatalogRecordType RecordType;
    public uint UnixSpecialField;

    public abstract int Size { get; }

    public virtual int ReadFrom(ReadOnlySpan<byte> buffer)
    {
        RecordType = (CatalogRecordType)EndianUtilities.ToInt16BigEndian(buffer);
        FileId = EndianUtilities.ToUInt32BigEndian(buffer.Slice(8));
        CreateTime = HfsPlusUtilities.ReadHFSPlusDate(DateTimeKind.Utc, buffer.Slice(12));
        ContentModifyTime = HfsPlusUtilities.ReadHFSPlusDate(DateTimeKind.Utc, buffer.Slice(16));
        AttributeModifyTime = HfsPlusUtilities.ReadHFSPlusDate(DateTimeKind.Utc, buffer.Slice(20));
        AccessTime = HfsPlusUtilities.ReadHFSPlusDate(DateTimeKind.Utc, buffer.Slice(24));
        BackupTime = HfsPlusUtilities.ReadHFSPlusDate(DateTimeKind.Utc, buffer.Slice(28));

        FileSystemInfo = HfsPlusUtilities.ReadBsdInfo(buffer.Slice(32), out var special);
        UnixSpecialField = special;

        return 0;
    }

    public abstract void WriteTo(Span<byte> buffer);
}