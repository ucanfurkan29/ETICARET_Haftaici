using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Category
    {
        public int Id { get; set; } // kategorinin benzersiz kimliği
        public string Name { get; set; } // kategorinin adı
        public List<ProductCategory> ProductCategories { get; set; } // Bu kategoriye ait ürünlerle ilişki
        public Category()
        {
            ProductCategories = new List<ProductCategory>(); // İlişki listesini başlat
        }
    }
}
