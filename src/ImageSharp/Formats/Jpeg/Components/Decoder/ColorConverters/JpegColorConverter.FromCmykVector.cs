// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters
{
    internal abstract partial class JpegColorConverterBase
    {
        internal sealed class FromCmykVector : JpegColorConverterVector
        {
            public FromCmykVector(int precision)
                : base(JpegColorSpace.Cmyk, precision)
            {
            }

            protected override void ConvertCoreVectorizedInplace(in ComponentValues values)
            {
                ref Vector<float> cBase =
                    ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component0));
                ref Vector<float> mBase =
                    ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component1));
                ref Vector<float> yBase =
                    ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component2));
                ref Vector<float> kBase =
                    ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component3));

                var scale = new Vector<float>(1 / (this.MaximumValue * this.MaximumValue));

                nint n = values.Component0.Length / Vector<float>.Count;
                for (nint i = 0; i < n; i++)
                {
                    ref Vector<float> c = ref Unsafe.Add(ref cBase, i);
                    ref Vector<float> m = ref Unsafe.Add(ref mBase, i);
                    ref Vector<float> y = ref Unsafe.Add(ref yBase, i);
                    Vector<float> k = Unsafe.Add(ref kBase, i);

                    k *= scale;
                    c *= k;
                    m *= k;
                    y *= k;
                }
            }

            protected override void ConvertCoreInplace(in ComponentValues values) =>
                FromCmykScalar.ConvertCoreInplace(values, this.MaximumValue);
        }
    }
}
