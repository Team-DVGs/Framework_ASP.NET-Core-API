﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Do_an_mon_hoc.Models;

public partial class SaleItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Display(AutoGenerateField = false)]
    public int? ProductId { get; set; }

    [Display(AutoGenerateField = false)]
    public int? EventId { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("EventId")]
    public virtual SaleEvent Event { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}