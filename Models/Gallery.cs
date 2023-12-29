﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Do_an_mon_hoc.Models;

public partial class Gallery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Thumbnail { get; set; }

    public int? Sort { get; set; }

    [Display(AutoGenerateField = false)]
    public int? ProductId { get; set; }

    public override string ToString()
    {
        return Product.Name + ": " + Sort;
    }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}