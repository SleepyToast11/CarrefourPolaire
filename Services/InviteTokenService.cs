using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Services;

public class InviteTokenService(EventContext context) : IInviteTokenService
{
    public async Task<GroupInviteToken> GetOrCreateTokenAsync(Guid registrationId)
    {
        var token = await context.GroupInviteTokens
            .FirstOrDefaultAsync(t => t.GroupId == registrationId && t.Active);
        if (token != null)
            return token;

        return await CreateNewTokenAsync(registrationId);
    }

    public async Task<GroupInviteToken> CreateNewTokenAsync(Guid registrationId)
    {
        // Invalidate old tokens
        var oldTokens = await context.GroupInviteTokens
            .Where(t => t.GroupId == registrationId && t.Active)
            .ToListAsync();
        
        Console.WriteLine("Hello, C#!");

        foreach (var t in oldTokens)
            t.Active = false;

        var newToken = new GroupInviteToken
        {
            GroupId = registrationId,
            Active = true,
        };
        Console.WriteLine(newToken.ToString());

        context.GroupInviteTokens.Add(newToken);
        await context.SaveChangesAsync();

        return newToken;
    }
}