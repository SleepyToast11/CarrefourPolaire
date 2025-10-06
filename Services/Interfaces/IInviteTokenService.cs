using CarrefourPolaire.Models;

namespace CarrefourPolaire.Services.Interfaces;

public interface IInviteTokenService
{
    Task<GroupInviteToken> GetOrCreateTokenAsync(Guid registrationId);
    Task<GroupInviteToken> CreateNewTokenAsync(Guid registrationId);
}