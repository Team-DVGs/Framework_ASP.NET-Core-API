﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Do_an_mon_hoc.Models;

public partial class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? Rating { get; set; }

    public string Title { get; set; }

    public string Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    [Display(AutoGenerateField = false)]
    public int? ProductId { get; set; }

    [Display(AutoGenerateField = false)]
    public int? UserId { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}