using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("product")]
public partial class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name", TypeName = "character varying")]
    public string? ProductName { get; set; }

    [Column("product_artist")]
    public int ProductArtist { get; set; }

    [Column("product_price")]
    public double? ProductPrice { get; set; }

    [Column("product_picture", TypeName = "character varying")]
    public string? ProductPicture { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("ProductArtist")]
    [InverseProperty("Products")]
    public virtual ArtistProfile ProductArtistNavigation { get; set; } = null!;
}
