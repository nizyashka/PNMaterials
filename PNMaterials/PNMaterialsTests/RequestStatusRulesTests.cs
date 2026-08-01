using Xunit;
using PNMaterialsDomain;
using PNMaterialsDomain.Entities;

namespace PNMaterialsTests;

public class RequestStatusRulesTests
{
    [Theory]
    [InlineData(RequestStatus.Created, RequestStatus.InWork)]     // СЗДН → ВРБТ
    [InlineData(RequestStatus.Created, RequestStatus.Deleted)]    // СЗДН → УДЛН
    [InlineData(RequestStatus.InWork, RequestStatus.Completed)]  // ВРБТ → ВПЛН
    [InlineData(RequestStatus.InWork, RequestStatus.Deleted)]    // ВРБТ → УДЛН
    [InlineData(RequestStatus.Completed, RequestStatus.InWork)]     // ВПЛН → ВРБТ
    [InlineData(RequestStatus.Completed, RequestStatus.Deleted)]    // ВПЛН → УДЛН
    [InlineData(RequestStatus.Deleted, RequestStatus.Created)]    // УДЛН → СЗДН
    public void CanTransition_AllowedPairs_ReturnsTrue(RequestStatus from, RequestStatus to)
    {
        Assert.True(RequestStatusRules.CanTransition(from, to));
    }

    [Theory]
    [InlineData(RequestStatus.Created, RequestStatus.Completed)]  // нельзя перепрыгнуть через «в работе»
    [InlineData(RequestStatus.InWork, RequestStatus.Created)]    // нельзя вернуться в «создана»
    [InlineData(RequestStatus.Completed, RequestStatus.Created)]    // нельзя из «закуплено» сразу в «создана»
    [InlineData(RequestStatus.Deleted, RequestStatus.InWork)]     // из удалённой — только в «создана»
    [InlineData(RequestStatus.Deleted, RequestStatus.Completed)]
    public void CanTransition_ForbiddenPairs_ReturnsFalse(RequestStatus from, RequestStatus to)
    {
        Assert.False(RequestStatusRules.CanTransition(from, to));
    }

    [Theory]
    [InlineData(RequestStatus.Created)]
    [InlineData(RequestStatus.InWork)]
    [InlineData(RequestStatus.Completed)]
    [InlineData(RequestStatus.Deleted)]
    public void CanTransition_ToSameStatus_ReturnsFalse(RequestStatus status)
    {
        Assert.False(RequestStatusRules.CanTransition(status, status));
    }
}