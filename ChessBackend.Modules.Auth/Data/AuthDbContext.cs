using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChessBackend.Modules.Auth.Entities;

namespace ChessBackend.Modules.Auth.Data;

public class AuthDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
}
