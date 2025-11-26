using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface ICategoryDal : IRepository<Category>
    {
        void DeleteFromCategory(int categoryId, int productId); // Belirtilen kategoriden belirtilen bir ürünü siler

        Category GetByIdWithProducts(int id); // Belirtilen ID'ye sahip kategoriyi ve ilişkili ürünleri getirir
    }
}
/* 
 Kategori yönetimi için gerekli veri erişim işlemlerini tanımlar.
 deleteFromCategory metodu, belirli bir ürünü kategoriden kaldırmak için kullanılır.
 getByIdWithProducts metodu, belirli bir kategoriye ait ürünleri almak için kullanılır.
 */
