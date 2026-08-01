using System.Linq;
using Xunit;
using PNMaterialsDomain;

namespace PNMaterialsTests;

public class MaterialCodeFormatterTests
{
    [Fact]
    public void Format_StartValue_ReturnsFirstCode()
    {
        var code = MaterialCodeFormatter.Format(10_000_000);
        Assert.Equal("10000000", code);
    }

    [Theory]
    [InlineData(10_000_000)]
    [InlineData(12_345_678)]
    [InlineData(19_999_999)]
    public void Format_ValidValue_ReturnsEightDigitsStartingWithOne(long value)
    {
        var code = MaterialCodeFormatter.Format(value);

        Assert.Equal(8, code.Length);
        Assert.StartsWith("1", code);
        Assert.True(code.All(char.IsDigit));
    }

    [Theory]
    [InlineData(9_999_999)]
    [InlineData(20_000_000)]
    [InlineData(0)]
    [InlineData(-5)]
    public void Format_OutOfRange_Throws(long value)
    {
        Assert.Throws<InvalidOperationException>(() => MaterialCodeFormatter.Format(value));
    }
}