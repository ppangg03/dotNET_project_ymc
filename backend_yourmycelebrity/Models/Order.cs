using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("order")]
public partial class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("order_date")]
    public DateOnly? OrderDate { get; set; }

    [Column("status", TypeName = "character varying")]
    public string? Status { get; set; }

    [Column("total_price")]
    public float? TotalPrice { get; set; }

    [Column("discount", TypeName = "character varying")]
    public string? Discount { get; set; }

    [Column("total_bill")]
    public float? TotalBill { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User User { get; set; } = null!;
}
