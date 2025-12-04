using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            _productService = productService;
        }
        [Route("products/{category?}")]
        public IActionResult List(string category, int page = 1)
        {
            const int pageSize = 5;

            //view model oluşturarak, sayfa bilgisi ve ürünleri ekle
            var products = new ProductListModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category), //kategoriye göre toplam ürün sayısı
                    ItemsPerPage = pageSize, //her sayfada gösterilecek ürün sayısı
                    CurrentCategory = category, //mevcut kategori
                    CurrentPage = page //mevcut sayfa
                },
                Products = _productService.GetProductByCategory(category, page, pageSize) //kategoriye göre ürünler
            };
            return View(products);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetail(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(new ProductDetailsModel()
            {
                Product = product, //ürün bilgisi
                Categories = product.ProductCategories.Select(i => i.Category).ToList(), //ürünün kategorileri
                Comments = product.Comments // ürünün yorumları
            });
        }
    }
}
