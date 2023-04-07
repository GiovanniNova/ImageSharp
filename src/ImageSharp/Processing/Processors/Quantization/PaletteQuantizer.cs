// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
/// Allows the quantization of images pixels using color palettes.
/// </summary>
public class PaletteQuantizer : IQuantizer
{
    private readonly ReadOnlyMemory<Color> colorPalette;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaletteQuantizer"/> class.
    /// </summary>
    /// <param name="palette">The color palette.</param>
    public PaletteQuantizer(ReadOnlyMemory<Color> palette)
        : this(palette, new QuantizerOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaletteQuantizer"/> class.
    /// </summary>
    /// <param name="palette">The color palette.</param>
    /// <param name="options">The quantizer options defining quantization rules.</param>
    public PaletteQuantizer(ReadOnlyMemory<Color> palette, QuantizerOptions options)
    {
        Guard.MustBeGreaterThan(palette.Length, 0, nameof(palette));
        Guard.NotNull(options, nameof(options));

        this.colorPalette = palette;
        this.Options = options;
    }

    /// <inheritdoc />
    public QuantizerOptions Options { get; }

    /// <inheritdoc />
    public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration)
        where TPixel : unmanaged, IPixel<TPixel>
        => this.CreatePixelSpecificQuantizer<TPixel>(configuration, this.Options);

    /// <inheritdoc />
    public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration, QuantizerOptions options)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(options, nameof(options));

        // Always use the palette length over options since the palette cannot be reduced.
        TPixel[] palette = new TPixel[this.colorPalette.Length];
        Color.ToPixel(this.colorPalette.Span, palette.AsSpan());
        return new PaletteQuantizer<TPixel>(configuration, options, palette);
    }
}
