using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreProductDal : EfCoreGenericRepository<Product, DataContext>, IProductDal
    {
        //kategoriye göre ürün sayısını döner
        public int GetCountByCategory(string category)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.AsQueryable(); //ürünleri sorgulanabilir hale getir
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    products = products.Include(i=>i.ProductCategories)
                        .ThenInclude(i=>i.Category) //ilişkili kategorileri dahil et
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower())); //kategori adına göre filtrele

                    return products.Count(); //ürün sayısını döner

                }
                else //tüm kategoriler için
                {
                    return products.Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category) //ilişkili kategorileri dahil et
                                    .Where(i=>i.ProductCategories.Any()) //kategorisi olan ürünleri filtrele
                                    .Count(); //ürün sayısını döner
                }
            }
        }

        //ürün detaylarını id ye göre döner
        public Product GetProductDetails(int id)
        {
            using (var context = new DataContext())
            {
                return context.Products
                    .Where(i=>i.Id == id)
                    .Include("Images") //ilişkili resimleri dahil et
                    .Include("Comments")
                    .Include(i => i.ProductCategories) //ilişkili kategorileri dahil et
                    .ThenInclude(i => i.Category) //kategori detaylarını da dahil et
                    .FirstOrDefault(); //ilk bulunan ürünü döner
            }
        }

        //kategoriye göre ürünleri sayfalı olarak döner
        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            using (var context =  new DataContext())
            {
                var products = context.Products.Include("Images").AsQueryable();
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(i => i.Category) //ilişkili kategorileri dahil et
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower())); //kategori adına göre filtrele
                }
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList(); //sayfalı ürün listesini döner
            }
        }

        //ürün güncelleme ve kategorileri ayarlama
        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.Include(i=>i.ProductCategories).FirstOrDefault(i=>i.Id == entity.Id); //ilgili ürünü ve kategorilerini getir

                if (products is not null)
                {
                    products.Price = entity.Price;
                    products.Name = entity.Name;
                    products.Description = entity.Description;
                    products.ProductCategories = categoryIds.Select(catid => new ProductCategory()
                    {
                        ProductId = entity.Id,
                        CategoryId = catid
                    }).ToList(); //yeni kategori ilişkilerini ayarla
                    products.Images = entity.Images;
                }
                context.SaveChanges(); //değişiklikleri kaydet
            }
        }

        public override void Delete(Product entity)
        {
            using (var context = new DataContext())
            {
                context.Images.RemoveRange(entity.Images); //ürüne ait tüm reismleri sil
                context.Products.Remove(entity); //ürünü sil
                context.SaveChanges(); //değişiklikleri kaydet
            }
        }

        //tüm ürünleri veya filtreye uyan ürünleri getir
        public override List<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            using (var context = new DataContext())
            {
                return filter == null 
                    ? context.Products.Include("Images").ToList() //filtre yoksa tüm ürünleri getir
                    : context.Products.Include("Images").Where(filter).ToList(); //filtreye uyan ürünleri getir
            }
        }
    }
}
