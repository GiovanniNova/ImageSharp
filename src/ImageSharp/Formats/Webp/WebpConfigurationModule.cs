// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.ImageSharp.Formats.Webp;

/// <summary>
/// Registers the image encoders, decoders and mime type detectors for the webp format.
/// </summary>
public sealed class WebpConfigurationModule : IImageFormatConfigurationModule
{
    /// <inheritdoc/>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(WebpFormat.Instance, WebpDecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(WebpFormat.Instance, new WebpEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new WebpImageFormatDetector());
    }
}
