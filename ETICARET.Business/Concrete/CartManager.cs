using ETICARET.Business.Abstract;
using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Concrete
{
    public class CartManager : ICartService
    {
        private ICartDal _cartDal;
        public CartManager(ICartDal cartDal) //dependency injection
        {
            _cartDal = cartDal;
        }
        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId); //kullanıcının sepetini al
            if (cart is not null) //sepet mevcutsa
            {
                var index = cart.CartItems.FindIndex(x => x.ProductId == productId);
                if (index < 0)
                {
                    //sepette ürün yoksa yeni ürün ekle
                    cart.CartItems.Add(
                        new CartItem
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            CartId = cart.Id
                        }
                    );
                }
                else//sepette ürün varsa miktarını güncelle
                {
                    cart.CartItems[index].Quantity += quantity;
                }
            }
            _cartDal.Update(cart); //sepeti güncelle
        }

        public void ClearCart(string cartId)
        {
            _cartDal.ClearCart(cartId);
        }

        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId); //kullanıcının sepetini al
            if (cart != null)
            {
                _cartDal.DeleteFromCart(cart.Id, productId); //sepetten ürünü sil
            }
        }

        public Cart GetCartByUserId(string userId) //kullanıcının sepetini getir
        {
            return _cartDal.GetCartByUserId(userId);
        }

        //yeni kullanıcı için boş sepet oluştur
        public void InitialCart(string userId)
        {
            Cart cart = new Cart
            {
                UserId = userId
            };
            _cartDal.Create(cart);
        }
    }
}
