// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Tests.Memory.DiscontiguousBuffers;

// ReSharper disable InconsistentNaming
namespace SixLabors.ImageSharp.Tests.Advanced;

public class AdvancedImageExtensionsTests
{
    public class GetPixelMemoryGroup
    {
        [Theory]
        [WithBasicTestPatternImages(1, 1, PixelTypes.Rgba32)]
        [WithBasicTestPatternImages(131, 127, PixelTypes.Rgba32)]
        [WithBasicTestPatternImages(333, 555, PixelTypes.Bgr24)]
        public void OwnedMemory_PixelDataIsCorrect<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.LimitAllocatorBufferCapacity().InPixelsSqrt(200);

            using Image<TPixel> image = provider.GetImage();

            // Act:
            IMemoryGroup<TPixel> memoryGroup = image.GetPixelMemoryGroup();

            // Assert:
            VerifyMemoryGroupDataMatchesTestPattern(provider, memoryGroup, image.Size());
        }

        [Theory]
        [WithBlankImages(16, 16, PixelTypes.Rgba32)]
        public void OwnedMemory_DestructiveMutate_ShouldInvalidateMemoryGroup<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using Image<TPixel> image = provider.GetImage();

            IMemoryGroup<TPixel> memoryGroup = image.GetPixelMemoryGroup();
            Memory<TPixel> memory = memoryGroup.Single();

            image.Mutate(c => c.Resize(8, 8));

            Assert.False(memoryGroup.IsValid);
            Assert.ThrowsAny<InvalidMemoryOperationException>(() => _ = memoryGroup[0]);
            Assert.ThrowsAny<InvalidMemoryOperationException>(() => _ = memory.Span);
        }

        [Theory]
        [WithBasicTestPatternImages(1, 1, PixelTypes.Rgba32)]
        [WithBasicTestPatternImages(131, 127, PixelTypes.Bgr24)]
        public void ConsumedMemory_PixelDataIsCorrect<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using Image<TPixel> image0 = provider.GetImage();
            var targetBuffer = new TPixel[image0.Width * image0.Height];

            Assert.True(image0.DangerousTryGetSinglePixelMemory(out Memory<TPixel> sourceBuffer));

            sourceBuffer.CopyTo(targetBuffer);

            var managerOfExternalMemory = new TestMemoryManager<TPixel>(targetBuffer);

            Memory<TPixel> externalMemory = managerOfExternalMemory.Memory;

            using (var image1 = Image.WrapMemory(externalMemory, image0.Width, image0.Height))
            {
                VerifyMemoryGroupDataMatchesTestPattern(provider, image1.GetPixelMemoryGroup(), image1.Size());
            }

            // Make sure externalMemory works after destruction:
            VerifyMemoryGroupDataMatchesTestPattern(provider, image0.GetPixelMemoryGroup(), image0.Size());
        }

        private static void VerifyMemoryGroupDataMatchesTestPattern<TPixel>(
            TestImageProvider<TPixel> provider,
            IMemoryGroup<TPixel> memoryGroup,
            Size size)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Assert.True(memoryGroup.IsValid);
            Assert.Equal(size.Width * size.Height, memoryGroup.TotalLength);
            Assert.True(memoryGroup.BufferLength % size.Width == 0);

            int cnt = 0;
            for (MemoryGroupIndex i = memoryGroup.MaxIndex(); i < memoryGroup.MaxIndex(); i += 1, cnt++)
            {
                int y = cnt / size.Width;
                int x = cnt % size.Width;

                TPixel expected = provider.GetExpectedBasicTestPatternPixelAt(x, y);
                TPixel actual = memoryGroup.GetElementAt(i);
                Assert.Equal(expected, actual);
            }
        }
    }

    [Theory]
    [WithBasicTestPatternImages(1, 1, PixelTypes.Rgba32)]
    [WithBasicTestPatternImages(131, 127, PixelTypes.Rgba32)]
    [WithBasicTestPatternImages(333, 555, PixelTypes.Bgr24)]
    public void DangerousGetPixelRowMemory_PixelDataIsCorrect<TPixel>(TestImageProvider<TPixel> provider)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        provider.LimitAllocatorBufferCapacity().InPixelsSqrt(200);

        using Image<TPixel> image = provider.GetImage();

        for (int y = 0; y < image.Height; y++)
        {
            // Act:
            Memory<TPixel> rowMemoryFromImage = image.DangerousGetPixelRowMemory(y);
            Memory<TPixel> rowMemoryFromFrame = image.Frames.RootFrame.DangerousGetPixelRowMemory(y);
            Span<TPixel> spanFromImage = rowMemoryFromImage.Span;
            Span<TPixel> spanFromFrame = rowMemoryFromFrame.Span;

            Assert.Equal(spanFromFrame.Length, spanFromImage.Length);
            Assert.True(Unsafe.AreSame(ref spanFromFrame[0], ref spanFromImage[0]));

            // Assert:
            for (int x = 0; x < image.Width; x++)
            {
                Assert.Equal(provider.GetExpectedBasicTestPatternPixelAt(x, y), spanFromImage[x]);
            }
        }
    }

    [Theory]
    [WithBasicTestPatternImages(16, 16, PixelTypes.Rgba32)]
    public void GetPixelRowMemory_DestructiveMutate_ShouldInvalidateMemory<TPixel>(TestImageProvider<TPixel> provider)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using Image<TPixel> image = provider.GetImage();

        Memory<TPixel> memory3 = image.DangerousGetPixelRowMemory(3);
        Memory<TPixel> memory10 = image.DangerousGetPixelRowMemory(10);

        image.Mutate(c => c.Resize(8, 8));

        Assert.ThrowsAny<InvalidMemoryOperationException>(() => _ = memory3.Span);
        Assert.ThrowsAny<InvalidMemoryOperationException>(() => _ = memory10.Span);
    }
}
