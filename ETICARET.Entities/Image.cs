using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    [Table("Images")] // Veritabanında "Images" tablosuna karşılık gelir
    public class Image
    {
        public int Id { get; set; } // Görüntünün benzersiz kimliği
        public string? ImageUrl { get; set; } // Görüntünün URL'si veya dosya yolu

        public int ProductId { get; set; } // Görüntünün ait olduğu ürünün kimliği
        public Product Product { get; set; } // Görüntünün ait olduğu ürün nesnesi ile ilişkisi
    }
}
