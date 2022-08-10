// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters
{
    internal abstract partial class JpegColorConverterBase
    {
        internal sealed class FromGrayScaleVector : JpegColorConverterVector
        {
            public FromGrayScaleVector(int precision)
                : base(JpegColorSpace.Grayscale, precision)
            {
            }

            protected override void ConvertCoreVectorizedInplace(in ComponentValues values)
            {
                ref Vector<float> cBase =
                    ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component0));

                var scale = new Vector<float>(1 / this.MaximumValue);

                nint n = values.Component0.Length / Vector<float>.Count;
                for (nint i = 0; i < n; i++)
                {
                    ref Vector<float> c0 = ref Unsafe.Add(ref cBase, i);
                    c0 *= scale;
                }
            }

            protected override void ConvertCoreInplace(in ComponentValues values) =>
                FromGrayscaleScalar.ConvertCoreInplace(values.Component0, this.MaximumValue);
        }
    }
}
