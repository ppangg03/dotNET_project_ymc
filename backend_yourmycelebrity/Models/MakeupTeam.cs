using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("makeup_team")]
public partial class MakeupTeam
{
    [Key]
    [Column("makeupteam_id")]
    public int MakeupteamId { get; set; }

    [Column("team_name", TypeName = "character varying")]
    public string? TeamName { get; set; }

    [Column("lead_id")]
    public int LeadId { get; set; }

    [Column("staff_id")]
    public int StaffId { get; set; }

    [ForeignKey("LeadId")]
    [InverseProperty("MakeupTeamLeads")]
    public virtual StaffProfile Lead { get; set; } = null!;

    [InverseProperty("MakeupTeam")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [ForeignKey("StaffId")]
    [InverseProperty("MakeupTeamStaffs")]
    public virtual StaffProfile Staff { get; set; } = null!;
}
