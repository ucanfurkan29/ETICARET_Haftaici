using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class ProductCategory
    {
        public int CategoryId { get; set; } // Kategorinin benzersiz kimliği
        public Category Category { get; set; } // Kategori nesnesi ile ilişkisi
        public int ProductId { get; set; } // Ürünün benzersiz kimliği
        public Product Product { get; set; } // Ürün nesnesi ile ilişkisi
    }
}
