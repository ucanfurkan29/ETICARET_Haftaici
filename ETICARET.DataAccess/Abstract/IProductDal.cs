using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface IProductDal : IRepository<Product>
    {
        int GetCountByCategory(string category); // Belirtilen kategoriye ait ürünlerin sayısını getirir
        Product GetProductDetails(int id); // Belirtilen ID'ye sahip ürünün detaylarını getirir
        List<Product> GetProductsByCategory(string category, int page, int pageSize); // Belirtilen kategoriye ait ürünleri sayfalı olarak getirir
        void Update(Product entity, int[] categoryIds); // Ürünü günceller ve ilişkili kategorileri ayarlar


    }
}
/*
bu arayüz, ürün yönetimi için gerekli veri erişim işlemlerini tanımlar. 
 */
