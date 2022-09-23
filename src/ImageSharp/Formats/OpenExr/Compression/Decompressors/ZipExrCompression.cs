// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Buffers;
using System.IO.Compression;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.OpenExr.Compression.Decompressors;

internal class ZipExrCompression : ExrBaseDecompressor
{
    private readonly IMemoryOwner<byte> tmpBuffer;

    public ZipExrCompression(MemoryAllocator allocator, uint uncompressedBytes)
        : base(allocator, uncompressedBytes) => this.tmpBuffer = allocator.Allocate<byte>((int)uncompressedBytes);

    public override void Decompress(BufferedReadStream stream, uint compressedBytes, Span<byte> buffer)
    {
        Span<byte> uncompressed = this.tmpBuffer.GetSpan();

        long pos = stream.Position;
        using ZlibInflateStream inflateStream = new(
                   stream,
                   () =>
                   {
                       int left = (int)(compressedBytes - (stream.Position - pos));
                       return left > 0 ? left : 0;
                   });
        inflateStream.AllocateNewBytes((int)this.UncompressedBytes, true);
        DeflateStream dataStream = inflateStream.CompressedStream;

        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int bytesRead = dataStream.Read(uncompressed, totalRead, buffer.Length - totalRead);
            if (bytesRead <= 0)
            {
                break;
            }

            totalRead += bytesRead;
        }

        if (totalRead == 0)
        {
            ExrThrowHelper.ThrowInvalidImageContentException("Could not read zip compressed image data!");
        }

        Reconstruct(uncompressed, (uint)totalRead);
        Interleave(uncompressed, (uint)totalRead, buffer);
    }

    protected override void Dispose(bool disposing) => this.tmpBuffer.Dispose();
}
