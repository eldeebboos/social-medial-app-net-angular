using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    [MaxLength(8)]
    public required string UserName { get; set; }
    [Required]
    [StringLength(8, MinimumLength = 4)]
    public required string Password { get; set; }
}
