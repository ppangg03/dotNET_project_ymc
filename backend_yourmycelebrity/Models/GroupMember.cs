using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("group_members")]
public partial class GroupMember
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Column("idol_id")]
    public int IdolId { get; set; }

    [Column("position", TypeName = "character varying")]
    public string? Position { get; set; }

    [Column("debut_date_in_group")]
    public DateOnly? DebutDateInGroup { get; set; }

    [Column("artist_id")]
    public int ArtistId { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("GroupMembers")]
    public virtual ArtistProfile Artist { get; set; } = null!;

    [ForeignKey("IdolId")]
    [InverseProperty("GroupMembers")]
    public virtual IdolGroup Idol { get; set; } = null!;
}
