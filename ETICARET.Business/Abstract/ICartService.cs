using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICartService
    {
        void InitialCart(string userId); // Kullanıcı için boş bir sepet oluşturur
        Cart GetCartByUserId(string userId); // Kullanıcının sepetini getirir
        void AddToCart(string userId, int productId, int quantity); // Sepete ürün ekler
        void DeleteFromCart(string userId, int productId); // Sepetten ürün siler
        void ClearCart(string cartId); // Sepeti tamamen temizler
        //sepet işlemelerini controllerdan bağımsız olarak yönetebilmek için servis katmanında tanımlıyoruz
        //kullanıcı giriş yaptığında boş bir sepet oluşturma ürün ekleme gibi işlemler burada tanımlanır.
    }
}
