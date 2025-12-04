using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface IProductService
    {
        Product GetById(int id); // ID'ye göre ürün getirir
        List<Product> GetAll(); // Tüm ürünleri getirir
        List<Product> GetProductByCategory(string category, int page, int pageSize); // sayfalama ile kategoriye göre ürünleri getirir
        Product GetProductDetail(int id); // Ürün detaylarını getirir
        void Create(Product entity); // Yeni ürün oluşturur
        void Update(Product entity, int[] categoryIds); //ürünü ve kategorilerini günceller
        void Delete(Product entity); // Ürünü siler
        int GetCountByCategory(string category); // Kategoriye göre ürün sayısını getirir
    }
}
