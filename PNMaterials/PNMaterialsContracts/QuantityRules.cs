namespace PNMaterialsContracts;

public static class QuantityRules
{
    public static decimal Minimum(bool allowsFractions) => allowsFractions ? 0.01m : 1m;
    public static decimal Maximum(bool allowsFractions) => allowsFractions ? 999_999.99m : 999_999m;

    public static decimal Round(decimal quantity, bool allowsFractions)
    {
        var decimals = allowsFractions ? 2 : 0;
        return Math.Round(quantity, decimals, MidpointRounding.AwayFromZero);
    }

    public static bool IsValid(decimal quantity, bool allowsFractions)
        => quantity >= Minimum(allowsFractions)
        && quantity <= Maximum(allowsFractions)
        && Round(quantity, allowsFractions) == quantity;
}
