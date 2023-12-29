﻿using System;
using System.Collections.Generic;

namespace Do_an_mon_hoc.Models;

public partial class CartDTO_Get
{
    public int Id { get; set; }

    public int? Quantity { get; set; }

    public double? Total { get; set; }

    public double? Saved { get; set; }


    public virtual ICollection<CartItemDTO_Get> CartItems { get; set; } = new List<CartItemDTO_Get>();

}
