namespace PNMaterialsContracts;

public static class PNRoles
{
    public const string Admin = "Admin";
    public const string ReadOnly = "ReadOnly";
    public const string ReadAndWrite = "ReadAndWrite";

    public static readonly IReadOnlyList<string> All = [Admin, ReadOnly, ReadAndWrite];
}
