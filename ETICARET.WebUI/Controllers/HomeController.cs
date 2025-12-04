using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ETICARET.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IProductService _productService;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            //ürünleri veritabanýndan al
            var products = _productService.GetAll();

            //eðer ürün yoksa boþ bir liste oluþtur
            if (products == null || !products.Any()) 
            {
                products = new List<Product>();
            }

            return View(new ProductListModel()
            {
                Products = products
            });
        }
    }
}
