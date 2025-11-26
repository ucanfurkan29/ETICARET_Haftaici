using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreCartDal : EfCoreGenericRepository<Cart, DataContext>, ICartDal
    {
        //belirtilen sepetteki tüm ürünleri siler
        public void ClearCart(string cartId)
        {
            using (var context = new DataContext())
            {
                var cmd = @"delete from CartItem where CartId=@p0";
                context.Database.ExecuteSqlRaw(cmd, cartId); //Raw sql komutu çalıştırmak için
            }
        }

        //sepette belirli bir ürünü siler
        public void DeleteFromCart(int cartId, int productId)
        {
            using (var context = new DataContext())
            {
                var cmd = @"delete from CartItem where CartId=@p0 and Product=@p1"; //sepet id ve ürün id ye göre sil
                context.Database.ExecuteSqlRaw(cmd, cartId,productId); //Raw sql komutu çalıştırmak için
            }
        }

        //kullanıcı id sine göre sepeti getirir
        public Cart GetCartByUserId(string userId)
        {
            using (var context = new DataContext())
            {
                return context.Carts
                    .Include(i => i.CartItems) //ilişkili cartitemleri dahil et
                    .ThenInclude(i => i.Product) //cartitem içindeki productları da dahil et
                    .ThenInclude(i => i.Images) //product içindeki image leri de dahil et
                    .FirstOrDefault(i => i.UserId == userId); //kullanıcı id sine göre getir
            }
        }

        public override void Update(Cart entity)
        {
            using (var context = new DataContext())
            {
                context.Carts.Update(entity); //ilgili sepeti güncelle
                context.SaveChanges();
            }
        }
    }
}
