using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("staff_profile")]
public partial class StaffProfile
{
    [Key]
    [Column("staff_id")]
    public int StaffId { get; set; }

    [Column("responsibilities", TypeName = "character varying")]
    public string? Responsibilities { get; set; }

    [Column("department", TypeName = "character varying")]
    public string? Department { get; set; }

    [InverseProperty("Lead")]
    public virtual ICollection<MakeupTeam> MakeupTeamLeads { get; set; } = new List<MakeupTeam>();

    [InverseProperty("Staff")]
    public virtual ICollection<MakeupTeam> MakeupTeamStaffs { get; set; } = new List<MakeupTeam>();

    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StaffProfiles")]
    public virtual User? User { get; set; }
}
