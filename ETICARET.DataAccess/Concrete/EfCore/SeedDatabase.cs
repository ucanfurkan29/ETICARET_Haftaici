using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class SeedDatabase
    {
        public static void Seed()
        {
            var context = new DataContext();

            //eğer veritabanında bekleyen migration yoksa
            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                //eğer kategori tablosu boşsa kategorileri ekle
                if (context.Categories.Count() == 0)
                {
                    context.AddRange(Categories);
                }

                //eğer ürün tablosu boşsa ürünleri ve ürün-kategori ilişkilerini ekle
                if (context.Products.Count() == 0)
                {
                    context.AddRange(Products);
                    context.AddRange(ProductCategories);
                }
                context.SaveChanges();
            }

        }

        private static Category[] Categories =
        {
            new Category(){ Name = "Telefon"},
            new Category(){ Name = "Bilgisayar"},
            new Category(){ Name = "Elektronik"},
            new Category(){ Name = "Ev Gereçleri"}
        };
        private static Product[] Products =
        {
            new Product(){ Name = "Samsung Note 8" , Price=15000, Images = {new Image() {ImageUrl = "samsung.jpg"}, new Image() {ImageUrl = "samsung2.jpg"}, new Image() {ImageUrl = "samsung3.jpg" }, new Image() {ImageUrl = "samsung4.jpg" } }, Description="<p>Güzel Telefon</p>"},
            new Product(){ Name = "Samsung Note 7" , Price = 6000, Images = { new Image() {ImageUrl = "samsung5.jpg" },  new Image() {ImageUrl = "samsung6.jpg" }, new Image() {ImageUrl = "samsung7.jpg" }, new Image() {ImageUrl = "samsung8.jpg" } },Description ="<p>Samsung Note 7 Farkı ile Tanışın</p>" },
            new Product(){ Name = "Samsung Note 8" , Price = 7000, Images = { new Image() {ImageUrl = "samsung9.jpg" },  new Image() {ImageUrl = "samsung10.jpg" }, new Image() {ImageUrl = "samsung1.jpg" }, new Image() {ImageUrl = "samsung4.jpg" } },Description ="<p>Samsung Note 8 ile Anı ölümsüzleştirin</p>" },

        };
        private static ProductCategory[] ProductCategories =
        {
            new ProductCategory(){ Product=Products[0], Category=Categories[0]},
            new ProductCategory(){ Product = Products[1],Category=Categories[3]},
            new ProductCategory(){ Product = Products[2],Category=Categories[0]},

        };
    }
}
