// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

/// <content>
/// Provides optimized overrides for bulk operations.
/// </content>
public partial struct Rgb24
{
    /// <summary>
    /// Provides optimized overrides for bulk operations.
    /// </summary>
    internal partial class PixelOperations : PixelOperations<Rgb24>
    {
        private static readonly Lazy<PixelTypeInfo> LazyInfo =
            new(() => PixelTypeInfo.Create<Rgb24>(PixelAlphaRepresentation.None), true);

        /// <inheritdoc />
        public override PixelTypeInfo GetPixelTypeInfo() => LazyInfo.Value;

        /// <inheritdoc />
        internal override void PackFromRgbPlanes(
            ReadOnlySpan<byte> redChannel,
            ReadOnlySpan<byte> greenChannel,
            ReadOnlySpan<byte> blueChannel,
            Span<Rgb24> destination)
        {
            int count = redChannel.Length;
            GuardPackFromRgbPlanes(greenChannel, blueChannel, destination, count);

            SimdUtils.PackFromRgbPlanes(redChannel, greenChannel, blueChannel, destination);
        }

        /// <inheritdoc />
        internal override void UnpackIntoRgbPlanes(
           Span<float> redChannel,
           Span<float> greenChannel,
           Span<float> blueChannel,
           ReadOnlySpan<Rgb24> source)
           => SimdUtils.UnpackToRgbPlanes(redChannel, greenChannel, blueChannel, source);
    }
}
