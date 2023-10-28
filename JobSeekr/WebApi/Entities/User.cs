﻿using WebApi.Enums;

namespace WebApi.Entities;

public class User
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public Role Role { get; set; }
}
