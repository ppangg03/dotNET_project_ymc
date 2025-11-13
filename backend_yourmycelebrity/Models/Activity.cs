using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("activities")]
public partial class Activity
{
    [Key]
    [Column("activity_id")]
    public int ActivityId { get; set; }

    [Column("partner_name", TypeName = "character varying")]
    public string? PartnerName { get; set; }

    [Column("activity_type", TypeName = "character varying")]
    public string? ActivityType { get; set; }

    [Column("title", TypeName = "character varying")]
    public string? Title { get; set; }

    [Column("description", TypeName = "character varying")]
    public string? Description { get; set; }

    [Column("contract_date")]
    public DateOnly? ContractDate { get; set; }

    [Column("appointment_date")]
    public DateOnly? AppointmentDate { get; set; }

    [Column("completion_date")]
    public DateOnly? CompletionDate { get; set; }

    [Column("status", TypeName = "character varying")]
    public string? Status { get; set; }

    [Column("created_at")]
    public DateOnly? CreatedAt { get; set; }

    [Column("update_at")]
    public DateOnly? UpdateAt { get; set; }

    [InverseProperty("Activity")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
