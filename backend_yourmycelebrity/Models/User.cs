using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("users")]
public partial class User
{
    [Column("username", TypeName = "character varying")]
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]
    public string? Username { get; set; }

    [Column("passwordHash", TypeName = "character varying")]
    [Required]
    public string? PasswordHash { get; set; }

    [Column("email", TypeName = "character varying")]
    [Required]
    public string? Email { get; set; }

    [Column("role", TypeName = "character varying")]
    public string Role { get; set; } = null!;

    [Column("first_name", TypeName = "character varying")]
    public string? FirstName { get; set; }

    [Column("last_name", TypeName = "character varying")]
    public string? LastName { get; set; }

    [Column("phone", TypeName = "character varying")]
    public string? Phone { get; set; }

    [Column("address", TypeName = "character varying")]
    public string? Address { get; set; }

    [Column("profile_pic", TypeName = "character varying")]
    public string? ProfilePic { get; set; }

    [Column("created_at")]
    public DateOnly? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateOnly? UpdatedAt { get; set; }

    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ArtistProfile> ArtistProfiles { get; set; } = new List<ArtistProfile>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<StaffProfile> StaffProfiles { get; set; } = new List<StaffProfile>();
}
