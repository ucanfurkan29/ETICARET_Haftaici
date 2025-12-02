using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICategoryService
    {
        Category GetById(int id); // ID'ye göre kategori getirir
        Category GetByWithProducts(int id); // ID'ye göre kategori ve ilişkili ürünleri getirir
        List<Category> GetAll(); // Tüm kategorileri getirir
        void Create(Category entity); // Yeni kategori oluşturur
        void Update(Category entity); // Mevcut kategoriyi günceller
        void Delete(Category entity); // Kategoriyi siler
        void DeleteFromCategory(int categoryId, int productId); // Kategoriden ürünü siler
    }
}
