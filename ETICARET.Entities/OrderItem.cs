using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } // Sipariş kaleminin benzersiz kimliği
        public int OrderId { get; set; } // Sipariş kaleminin ait olduğu siparişin kimliği
        public Order Order { get; set; } // Sipariş kaleminin ait olduğu sipariş nesnesi ile ilişkisi
        public int ProductId { get; set; } // Sipariş kaleminin ait olduğu ürünün kimliği
        public Product Product { get; set; } // Sipariş kaleminin ait olduğu ürün nesnesi ile ilişkisi
        public int Quantity { get; set; } // Sipariş kalemindeki ürün adedi
        public decimal Price { get; set; } // Sipariş kalemindeki ürünün fiyatı
    }
}
