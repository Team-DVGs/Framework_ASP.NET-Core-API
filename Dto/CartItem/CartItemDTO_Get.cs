using System;
using System.Collections.Generic;

namespace Do_an_mon_hoc.Models;

public partial class CartItemDTO_Get
{
    public int Id { get; set; }

    public int? Quantity { get; set; }

    public int? Total { get; set; }

    public int? cartid { get; set; }

    public int? productid { get; set; }

    public string name {  get; set; }

    public double reg_price {  get; set; }

    public double discount_price { get; set; }

    //public int? user_id {  get; set; }

    //public virtual Cart? Cart { get; set; }



    //public virtual Product? Product { get; set; }
}
