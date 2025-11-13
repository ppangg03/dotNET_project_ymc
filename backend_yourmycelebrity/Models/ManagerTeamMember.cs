using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Keyless]
[Table("manager_team_members")]
public partial class ManagerTeamMember
{
    [Column("manager_team_id")]
    public int ManagerTeamId { get; set; }

    [Column("staff_id")]
    public int StaffId { get; set; }

    [Column("role_in_team", TypeName = "character varying")]
    public string? RoleInTeam { get; set; }

    [Column("join_at")]
    public DateOnly? JoinAt { get; set; }

    [ForeignKey("ManagerTeamId")]
    public virtual ManagerTeam ManagerTeam { get; set; } = null!;

    [ForeignKey("StaffId")]
    public virtual StaffProfile Staff { get; set; } = null!;
}
