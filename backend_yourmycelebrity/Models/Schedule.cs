using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("schedule")]
public partial class Schedule
{
    [Column("exhibit_location", TypeName = "character varying")]
    public string? ExhibitLocation { get; set; }

    [Column("official_time")]
    public TimeOnly? OfficialTime { get; set; }

    [Column("description_for_work", TypeName = "character varying")]
    public string? DescriptionForWork { get; set; }

    [Column("makeup_location", TypeName = "character varying")]
    public string? MakeupLocation { get; set; }

    [Column("description_for_staff", TypeName = "character varying")]
    public string? DescriptionForStaff { get; set; }

    [Column("artist_id")]
    public int ArtistId { get; set; }

    [Column("makeup_team_id")]
    public int MakeupTeamId { get; set; }

    [Column("activity_id")]
    public int ActivityId { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Key]
    [Column("schedule_id")]
    public int ScheduleId { get; set; }

    [ForeignKey("ActivityId")]
    [InverseProperty("Schedules")]
    public virtual Activity Activity { get; set; } = null!;

    [ForeignKey("ArtistId")]
    [InverseProperty("Schedules")]
    public virtual ArtistProfile Artist { get; set; } = null!;

    [ForeignKey("MakeupTeamId")]
    [InverseProperty("Schedules")]
    public virtual MakeupTeam MakeupTeam { get; set; } = null!;
}
