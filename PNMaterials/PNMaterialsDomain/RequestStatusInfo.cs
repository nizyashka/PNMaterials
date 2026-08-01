using PNMaterialsDomain.Entities;

namespace PNMaterialsDomain;

public static class RequestStatusInfo
{
    private static readonly Dictionary<RequestStatus, (string Code, string Name)> Info = new()
    {
        [RequestStatus.Created] = ("СЗДН", "Создана, но не передана в отдел снабжения"),
        [RequestStatus.InWork] = ("ВРБТ", "Запущена в работу (передана в отдел снабжения на закупку)"),
        [RequestStatus.Completed] = ("ВПЛН", "Закупка осуществлена"),
        [RequestStatus.Deleted] = ("УДЛН", "Заявка удалена")
    };

    public static string GetCode(RequestStatus status) => Info[status].Code;
    public static string GetName(RequestStatus status) => Info[status].Name;
}