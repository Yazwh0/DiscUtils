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

namespace BitMagic.DiscUtils.Vdi;

public class GeometryRecord
{
    public int Cylinders;
    public int Heads;
    public int Sectors;
    public int SectorSize;

    public GeometryRecord()
    {
        SectorSize = 512;
    }

    public static GeometryRecord FromCapacity(long capacity)
    {
        var result = new GeometryRecord();

        var totalSectors = capacity / 512;
        if (totalSectors / (16 * 63) <= 1024)
        {
            result.Cylinders = (int)Math.Max(totalSectors / (16 * 63), 1);
            result.Heads = 16;
        }
        else if (totalSectors / (32 * 63) <= 1024)
        {
            result.Cylinders = (int)Math.Max(totalSectors / (32 * 63), 1);
            result.Heads = 32;
        }
        else if (totalSectors / (64 * 63) <= 1024)
        {
            result.Cylinders = (int)(totalSectors / (64 * 63));
            result.Heads = 64;
        }
        else if (totalSectors / (128 * 63) <= 1024)
        {
            result.Cylinders = (int)(totalSectors / (128 * 63));
            result.Heads = 128;
        }
        else
        {
            result.Cylinders = (int)Math.Min(totalSectors / (255 * 63), 1024);
            result.Heads = 255;
        }

        result.Sectors = 63;

        return result;
    }

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Cylinders = EndianUtilities.ToInt32LittleEndian(buffer.Slice(0));
        Heads = EndianUtilities.ToInt32LittleEndian(buffer.Slice(4));
        Sectors = EndianUtilities.ToInt32LittleEndian(buffer.Slice(8));
        SectorSize = EndianUtilities.ToInt32LittleEndian(buffer.Slice(12));
    }

    public void Write(Span<byte> buffer)
    {
        EndianUtilities.WriteBytesLittleEndian(Cylinders, buffer.Slice(0));
        EndianUtilities.WriteBytesLittleEndian(Heads, buffer.Slice(4));
        EndianUtilities.WriteBytesLittleEndian(Sectors, buffer.Slice(8));
        EndianUtilities.WriteBytesLittleEndian(SectorSize, buffer.Slice(12));
    }

    public Geometry ToGeometry(long actualCapacity)
    {
        var cylinderCapacity = SectorSize * (long)Sectors * Heads;
        return new Geometry((int)(actualCapacity / cylinderCapacity), Heads, Sectors, SectorSize);
    }
}