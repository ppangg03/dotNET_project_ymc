using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("artist_profile")]
public partial class ArtistProfile
{
    [Key]
    [Column("artist_id")]
    public int ArtistId { get; set; }

    [Column("official_name", TypeName = "character varying")]
    public string? OfficialName { get; set; }

    [Column("birthdate")]
    public DateOnly? Birthdate { get; set; }

    [Column("type", TypeName = "character varying")]
    public string? Type { get; set; }

    [Column("real_name", TypeName = "character varying")]
    public string? RealName { get; set; }

    [Column("mbti", TypeName = "character varying")]
    public string? Mbti { get; set; }

    [Column("bloodtype", TypeName = "character varying")]
    public string? Bloodtype { get; set; }

    [Column("debut_date")]
    public DateOnly? DebutDate { get; set; }

    [Column("picture", TypeName = "character varying")]
    public string? Picture { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("native_real_name", TypeName = "character varying")]
    public string? NativeRealName { get; set; }

    [Column("height_cm", TypeName = "character varying")]
    public string? HeightCm { get; set; }

    [Column("weight_kg", TypeName = "character varying")]
    public string? WeightKg { get; set; }

    [Column("nationality", TypeName = "character varying")]
    public string? Nationality { get; set; }

    [Column("native_offcial_name", TypeName = "character varying")]
    public string? NativeOffcialName { get; set; }

    [InverseProperty("Artist")]
    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    [InverseProperty("ProductArtistNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("Artist")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [ForeignKey("UserId")]
    [InverseProperty("ArtistProfiles")]
    public virtual User User { get; set; } = null!;
}
