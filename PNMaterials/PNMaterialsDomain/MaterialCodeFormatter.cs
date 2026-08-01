namespace PNMaterialsDomain;

public static class MaterialCodeFormatter
{
    public const long Start = 10_000_000;
    public const long End = 19_999_999;

    public static string Format(long sequenceValue)
    {
        if (sequenceValue < Start || sequenceValue > End)
            throw new InvalidOperationException($"Значение {sequenceValue} вне диапазона кодов материалов [{Start}..{End}].");

        return sequenceValue.ToString();
    }
}