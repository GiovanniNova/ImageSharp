// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.ImageSharp.Formats;

/// <summary>
/// Encapsulates methods used for detecting the raw image information without fully decoding it.
/// </summary>
public interface IImageInfoDetector
{
    /// <summary>
    /// Reads the raw image information from the specified stream.
    /// </summary>
    /// <remarks>
    /// This method is designed to support the ImageSharp internal infrastructure and is not recommended for direct use.
    /// </remarks>
    /// <param name="options">The general decoder options.</param>
    /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="IImageInfo"/> object.</returns>
    /// <exception cref="ImageFormatException">Thrown if the encoded image contains errors.</exception>
    IImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken);
}
