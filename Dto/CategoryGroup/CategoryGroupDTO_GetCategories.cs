using Do_an_mon_hoc.Dto.Products;
using Do_an_mon_hoc.Models;

namespace Do_an_mon_hoc.Dto.CategoryGroup
{
    public class CategoryGroupDTO_GetCategories
    {
        public string name { get; set; } = null!;


        public virtual ICollection<CategoryDTO_Get> categories { get; set; } = new List<CategoryDTO_Get>();
    }
}
