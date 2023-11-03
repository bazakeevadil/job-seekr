﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities;

public class User
{
    public long Id { get; set; }

    [StringLength(150)]
    public required string Email { get; set; }

    [StringLength(200)]
    public required string HashPassword { get; set; }
    public required bool IsBlocked { get; set; }
    public Role Role { get; set; }

    public List<Resume> Resumes { get; set; } = new();
}
