using System.Linq;
using PNMaterialsDomain.Entities;

namespace PNMaterialsDomain;

public static class RequestStatusRules
{
    private static readonly Dictionary<RequestStatus, RequestStatus[]> Allowed = new()
    {
        [RequestStatus.Created] = [RequestStatus.InWork, RequestStatus.Deleted],
        [RequestStatus.InWork] = [RequestStatus.Completed, RequestStatus.Deleted],
        [RequestStatus.Completed] = [RequestStatus.InWork, RequestStatus.Deleted],
        [RequestStatus.Deleted] = [RequestStatus.Created]
    };

    public static bool CanTransition(RequestStatus from, RequestStatus to)
        => Allowed.TryGetValue(from, out var targets) && targets.Contains(to);

    public static IReadOnlyList<RequestStatus> AllowedTargets(RequestStatus from)
        => Allowed.TryGetValue(from, out var targets) ? targets : Array.Empty<RequestStatus>();
}