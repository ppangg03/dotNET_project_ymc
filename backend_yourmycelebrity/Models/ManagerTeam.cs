using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("manager_team")]
public partial class ManagerTeam
{
    [Key]
    [Column("manager_team_id")]
    public int ManagerTeamId { get; set; }

    [Column("team_name", TypeName = "character varying")]
    public string? TeamName { get; set; }

    [Column("description", TypeName = "character varying")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateOnly? CreatedAt { get; set; }

    [Column("update_at")]
    public DateOnly? UpdateAt { get; set; }
}
