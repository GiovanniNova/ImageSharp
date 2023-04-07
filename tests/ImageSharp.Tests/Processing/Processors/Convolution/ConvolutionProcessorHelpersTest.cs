// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Tests.Processing.Processors.Convolution;

[GroupOutput("Convolution")]
public class ConvolutionProcessorHelpersTest
{
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(9)]
    [InlineData(22)]
    [InlineData(33)]
    [InlineData(80)]
    public void VerifyGaussianKernelDecomposition(int radius)
    {
        int kernelSize = (radius * 2) + 1;
        float sigma = radius / 3F;
        float[] kernel = ConvolutionProcessorHelpers.CreateGaussianBlurKernel(kernelSize, sigma);
        DenseMatrix<float> matrix = DotProduct(kernel, kernel);

        bool result = matrix.TryGetLinearlySeparableComponents(out float[] row, out float[] column);

        Assert.True(result);
        Assert.NotNull(row);
        Assert.NotNull(column);
        Assert.Equal(row.Length, matrix.Rows);
        Assert.Equal(column.Length, matrix.Columns);

        float[,] dotProduct = DotProduct(row, column);

        for (int y = 0; y < column.Length; y++)
        {
            for (int x = 0; x < row.Length; x++)
            {
                Assert.True(Math.Abs(matrix[y, x] - dotProduct[y, x]) < 0.0001F);
            }
        }
    }

    [Fact]
    public void VerifyNonSeparableMatrix()
    {
        bool result = LaplacianKernels.LaplacianOfGaussianXY.TryGetLinearlySeparableComponents(
            out float[] row,
            out float[] column);

        Assert.False(result);
        Assert.Null(row);
        Assert.Null(column);
    }

    private static DenseMatrix<float> DotProduct(float[] row, float[] column)
    {
        float[,] matrix = new float[column.Length, row.Length];

        for (int x = 0; x < row.Length; x++)
        {
            for (int y = 0; y < column.Length; y++)
            {
                matrix[y, x] = row[x] * column[y];
            }
        }

        return matrix;
    }
}
