using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface ICartDal : IRepository<Cart> // IRepository arayüzünden türetilir ve Cart varlığına özgü veri erişim işlemlerini tanımlar
    {
        void ClearCart(string cartId); // Belirtilen sepet kimliğine sahip sepeti temizler
        void DeleteFromCart(int cartId, int productId); // belirtilen sepetin içinden belirli bir ürünü siler

        Cart GetCartByUserId(string userId); // Belirtilen kullanıcı kimliğine sahip sepeti getirir
    }
}
/*
 Sepet yönetimi için gerekli veri erişim işlemlerini tanımlar.
clearCart metodu, bir kullanıcının sepetini tamamen boşaltmak için kullanılır.
deleteFromCart metodu, belirli bir ürünü kullanıcının sepetinden kaldırmak için kullanılır.
getCartByUserId metodu, belirli bir kullanıcıya ait sepeti almak için kullanılır.
 */
