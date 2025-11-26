using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Product
    {
        public int Id { get; set; } // Ürünün benzersiz kimliği
        public string Name { get; set; } // Ürünün adı
        public string Description { get; set; } // Ürünün açıklaması
        public List<Image> Images { get; set; } // Ürüne ait görüntülerin listesi

        [Range(0, double.MaxValue, ErrorMessage = "Fiyat geçerli bir değer olmalı.")]
        public decimal Price { get; set; } // Ürünün fiyatı

        public List<ProductCategory> ProductCategories { get; set; } // Ürünün ait olduğu kategorilerle ilişki
        public List<Comment> Comments { get; set; } // Ürüne ait yorumların listesi

        public Product() // Varsayılan yapıcı
        {
            Images = new List<Image>();
            ProductCategories = new List<ProductCategory>();
            Comments = new List<Comment>();
        }

    }
}
