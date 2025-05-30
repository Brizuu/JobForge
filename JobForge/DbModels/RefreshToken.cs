﻿using JobForge.Models;

namespace JobForge.DbModels;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}