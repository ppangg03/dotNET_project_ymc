using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend_yourmycelebrity.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Models;

[Table("security_token")]
public partial class SecurityToken
{
    [Key]
    [Column("token_id")]
    public int TokenId { get; set; }

    [Column("email", TypeName = "character varying")]
    public string? Email { get; set; }


    [Required]
    [Column("token", TypeName = "character varying")]
    [StringLength(500)]
    public string? Token { get; set; } = null!;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("expires_at", TypeName = "timestamp without time zone")]
    public DateTime? ExpiresAt { get; set; }

    [Column("is_used")]
    public bool? IsUsed { get; set; }

    [Column("is_verified")]
    public bool? IsVerified { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// Type: OTP, PasswordReset, Blacklist
    /// </summary>
    [Column("token_type", TypeName = "character varying")]
    public TokenType? TokenType { get; set; } = null!;

    /// <summary>
    /// Purpose: Register, PasswordReset, Logout, etc.
    /// </summary>
    [Column("purpose", TypeName = "character varying")]
    public TokenPurpose? Purpose { get; set; }

    [Column("blacklisted_at", TypeName = "timestamp without time zone")]
    public DateTime? BlacklistedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("SecurityTokens")]
    public virtual User? User { get; set; }
}
