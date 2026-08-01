using System.Linq;
using Xunit;
using PNMaterialsDomain;

namespace PNMaterialsTests;

public class RequestNumberFormatterTests
{
    [Fact]
    public void Format_StartValue_ReturnsFirstNumber()
    {
        var number = RequestNumberFormatter.Format(5_000_000_000);
        Assert.Equal("5000000000", number);
    }

    [Theory]
    [InlineData(5_000_000_000)]
    [InlineData(5_123_456_789)]
    [InlineData(9_999_999_999)]
    public void Format_ValidValue_ReturnsTenDigits(long value)
    {
        var number = RequestNumberFormatter.Format(value);

        Assert.Equal(10, number.Length);
        Assert.True(number.All(char.IsDigit));
    }

    [Theory]
    [InlineData(4_999_999_999)]
    [InlineData(10_000_000_000)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Format_OutOfRange_Throws(long value)
    {
        Assert.Throws<InvalidOperationException>(() => RequestNumberFormatter.Format(value));
    }
}