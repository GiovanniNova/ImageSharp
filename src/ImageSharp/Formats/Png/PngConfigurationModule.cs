// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
/// Registers the image encoders, decoders and mime type detectors for the png format.
/// </summary>
public sealed class PngConfigurationModule : IImageFormatConfigurationModule
{
    /// <inheritdoc/>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetEncoder(PngFormat.Instance, new PngEncoder());
        configuration.ImageFormatsManager.SetDecoder(PngFormat.Instance, PngDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new PngImageFormatDetector());
    }
}
