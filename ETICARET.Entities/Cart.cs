using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Cart
    {
        public int Id { get; set; } // sepetin benzersiz kimliği
        public string UserId { get; set; } // sepete ait kullanıcı kimliği
        public List<CartItem> CartItems { get; set; } // sepetteki öğelerin listesi
    }
    public class CartItem
    {
        public int Id { get; set; }  // sepet öğesinin benzersiz kimliği
        public int ProductId { get; set; } // sepete eklenen ürünün kimliği
        public Product Product { get; set; } //ürünün nesne ile ilişkisi
        public Cart Cart { get; set; } //sepet nesne ile ilişkisi
        public int CartId { get; set; }  //Sepet kimliği 
        public int Quantity { get; set; } //üründen kaç tane sepete eklendi
    }
}
