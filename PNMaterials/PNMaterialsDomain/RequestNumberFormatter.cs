namespace PNMaterialsDomain;

public static class RequestNumberFormatter
{
    public const long Start = 5_000_000_000;
    public const long End = 9_999_999_999;

    public static string Format(long sequenceValue)
    {
        if (sequenceValue < Start || sequenceValue > End)
            throw new InvalidOperationException($"Значение {sequenceValue} вне диапазона номеров заявок [{Start}..{End}].");

        return sequenceValue.ToString();
    }
}