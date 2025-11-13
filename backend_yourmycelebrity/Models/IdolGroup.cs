using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("idol_group")]
[Index("IdolGroupId", Name = "artist_group_artist_id_idx")]
public partial class IdolGroup
{
    [Key]
    [Column("idol_group_id")]
    public int IdolGroupId { get; set; }

    [Column("group_name", TypeName = "character varying")]
    public string? GroupName { get; set; }

    [Column("debutdate")]
    public DateOnly? Debutdate { get; set; }

    [Column("fandom_name", TypeName = "character varying")]
    public string? FandomName { get; set; }

    [Column("members")]
    public int? Members { get; set; }

    [Column("picture", TypeName = "character varying")]
    public string? Picture { get; set; }

    [Column("group_name_korean", TypeName = "character varying")]
    public string? GroupNameKorean { get; set; }

    [Column("group_name_japanese", TypeName = "character varying")]
    public string? GroupNameJapanese { get; set; }

    [InverseProperty("Idol")]
    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
