// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.ImageSharp.Formats.Bmp;

/// <summary>
/// Registers the image encoders, decoders and mime type detectors for the bmp format.
/// </summary>
public sealed class BmpConfigurationModule : IImageFormatConfigurationModule
{
    /// <inheritdoc/>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetEncoder(BmpFormat.Instance, new BmpEncoder());
        configuration.ImageFormatsManager.SetDecoder(BmpFormat.Instance, BmpDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new BmpImageFormatDetector());
    }
}
