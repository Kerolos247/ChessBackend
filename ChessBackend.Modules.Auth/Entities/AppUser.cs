using System;
using Microsoft.AspNetCore.Identity;

namespace ChessBackend.Modules.Auth.Entities;

public class AppUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
