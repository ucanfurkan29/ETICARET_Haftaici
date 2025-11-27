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
    public class EfCoreCategoryDal : EfCoreGenericRepository<Category, DataContext>, ICategoryDal
    {
        //bir ürünü belirli bir kategoriden kaldırır
        public void DeleteFromCategory(int categoryId, int productId)
        {
            using (var context = new DataContext())
            {
                var cmd = @"delete from ProductCategory where ProductId=@p1 and CategoryId=@p0"; //kategori id ve ürün id ye göre sil
                context.Database.ExecuteSqlRaw(cmd, categoryId, productId); //Raw sql komutu çalıştırmak için
            }
        }


        //kategori bilgisiyle birlikte o kategoriye ait ürünleri getirir
        public Category GetByIdWithProducts(int id)
        {
            using (var context = new DataContext())
            {
                return context.Categories
                    .Where(i=> i.Id==id) //belirtilen id ye sahip kategoriyi bul
                    .Include(i => i.ProductCategories) //ilişkili productcategory leri dahil et
                    .ThenInclude(i => i.Product) //productcategory içindeki productları da dahil et
                    .ThenInclude(i => i.Images) //product içindeki image leri de dahil et
                    .FirstOrDefault(); //ilk bulunanı getir
            }
        }

        //kategori silme işlemi
        public override void Delete(Category entity)
        {
            using (var context = new DataContext())
            {
                context.Categories.Remove(entity); //Dbset üzerinden erişip İlgili entity i sil
                context.SaveChanges();
            }
        }
    }
}
