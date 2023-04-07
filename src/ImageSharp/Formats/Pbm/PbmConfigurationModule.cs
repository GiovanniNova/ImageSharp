// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.ImageSharp.Formats.Pbm;

/// <summary>
/// Registers the image encoders, decoders and mime type detectors for the Pbm format.
/// </summary>
public sealed class PbmConfigurationModule : IImageFormatConfigurationModule
{
    /// <inheritdoc/>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetEncoder(PbmFormat.Instance, new PbmEncoder());
        configuration.ImageFormatsManager.SetDecoder(PbmFormat.Instance, PbmDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new PbmImageFormatDetector());
    }
}
