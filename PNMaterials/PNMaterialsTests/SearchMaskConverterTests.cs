using Xunit;
using PNMaterialsDomain;

namespace PNMaterialsTests;

public class SearchMaskConverterTests
{
    [Theory]
    [InlineData("Кабель*", "Кабель%")]  // звёздочка в конце
    [InlineData("*ВВГ*", "%ВВГ%")]  // с обеих сторон
    [InlineData("Кабель*2.5", "Кабель%2.5")]    // в середине
    [InlineData("**", "%%")]    // несколько подряд
    public void ToLikePattern_Star_BecomesPercent(string mask, string expected)
    {
        Assert.Equal(expected, SearchMaskConverter.ToLikePattern(mask));
    }

    [Fact]
    public void ToLikePattern_NoStar_StaysExact()
    {
        Assert.Equal("Кабель", SearchMaskConverter.ToLikePattern("Кабель"));
    }

    [Theory]
    [InlineData("50%", @"50\%")]    // процент ищется буквально
    [InlineData("A_B", @"A\_B")]    // подчёркивание тоже
    [InlineData("A[B", @"A\[B")]    // открывающая скобка
    [InlineData(@"A\B", @"A\\B")]   // сам экранирующий символ
    public void ToLikePattern_SpecialChars_AreEscaped(string mask, string expected)
    {
        Assert.Equal(expected, SearchMaskConverter.ToLikePattern(mask));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToLikePattern_Empty_MatchesEverything(string? mask)
    {
        Assert.Equal("%", SearchMaskConverter.ToLikePattern(mask));
    }

    [Fact]
    public void ToLikePattern_TrimsSpaces()
    {
        Assert.Equal("Кабель%", SearchMaskConverter.ToLikePattern("  Кабель*  "));
    }
}