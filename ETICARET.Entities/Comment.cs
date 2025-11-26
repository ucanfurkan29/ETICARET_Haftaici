using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Comment
    {
        public int Id { get; set; }// Yorumun benzersiz kimliği
        public string Text { get; set; } // Yorumun metni
        public int ProductId { get; set; } // Yorumun ait olduğu ürünün kimliği
        public Product Product { get; set; } // Yorumun ait olduğu ürün nesnesi ile ilişkisi
        public string UserId { get; set; } // Yorumun ait olduğu kullanıcının kimliği
        public DateTime CreatedOn { get; set; } // Yorumun oluşturulma tarihi
    }
}
