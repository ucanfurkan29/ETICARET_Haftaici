using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ETICARET.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        //servisleri ve managerları constructor ile enjekte ediyoruz
        public AdminController(IProductService productService, ICategoryService categoryService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Route("admin/products")]
        public IActionResult ProductList()
        {
            //ürünlerin tümünü al ve productlist view'ine gönder
            return View(
                new ProductListModel()
                {
                    Products = _productService.GetAll()
                }
            );
        }

        public IActionResult CreateProduct()
        {
            var category = _categoryService.GetAll();

            ViewBag.Category = category.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            return View(new ProductModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model, List<IFormFile> files)
        {
            ModelState.Remove("SelectedCategories");

            if (ModelState.IsValid)
            {
                if (int.Parse(model.CategoryId) == -1)
                {
                    ModelState.AddModelError("", "Lütfen bir kategori seçiniz.");

                    ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    return View(model);
                }

                var entity = new Product()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price
                };
                if (files.Count < 4 || files == null)
                {
                    ModelState.AddModelError("", "Lütfen en az 4 resim yükleyin.");
                    ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    return View(model);
                }
                foreach (var item in files)
                {
                    Image image = new Image();
                    image.ImageUrl = item.FileName; //resmin yolunu alıyoruz

                    entity.Images.Add(image); //resmi ürüne ekliyoruz

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.FileName); //resmin kaydedileceği yolu belirliyoruz

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream); //resmi belirtilen yola kaydediyoruz
                    }
                }
                entity.ProductCategories = new List<ProductCategory>()
                {
                    new ProductCategory(){CategoryId = int.Parse(model.CategoryId), ProductId = entity.Id}
                };
                _productService.Create(entity); //ürünü oluşturuyoruz
                return RedirectToAction("ProductList");
            }
            ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            return View(model);
        }

        public IActionResult EditProduct(int id)
        {
            if (id == null) //id null ise 404 döner
            {
                return NotFound();
            }

            var entity = _productService.GetProductDetail(id);

            if (entity == null) //ürün bulunamazsa 404 döner
            {
                return NotFound();
            }

            var model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Images = entity.Images,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, List<IFormFile> files, int[] categoryIds)
        {
            var entity = _productService.GetById(model.Id);

            if (entity == null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.Images = model.Images;

            if (files != null && files.Count > 0)
            {
                foreach (var item in files)
                {
                    Image image = new Image();
                    image.ImageUrl = item.FileName; //resmin yolunu alıyoruz
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.FileName); //resmin kaydedileceği yolu belirliyoruz

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream); //resmi belirtilen yola kaydediyoruz
                    }
                }
            }

            _productService.Update(entity, categoryIds);
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);
            if (entity != null)
            {
                _productService.Delete(entity);
            }
            return RedirectToAction("ProductList");
        }

        public IActionResult CategoryList()
        {
            return View(
                new CategoryListModel()
                {
                    Categories = _categoryService.GetAll()
                }
            );
        }

        public IActionResult EditCategory(int? id)
        {
            var entity = _categoryService.GetByWithProducts(id.Value);

            return View(
                new CategoryModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Products = entity.ProductCategories.Select(i => i.Product).ToList()
                }
            );
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.Id); //mevcut kategoriyi id ile bul

            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name; //kategorinin ismini güncelle
            _categoryService.Update(entity); //kategoriyi güncelle
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteCategory(int categoryId)
        {
            var entity = _categoryService.GetById(categoryId);
            if (entity != null)
            {
                _categoryService.Delete(entity);
            }
            return RedirectToAction("CategoryList");
        }
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            var entity = new Category()
            {
                Name = model.Name
            };
            _categoryService.Create(entity);
            return RedirectToAction("CategoryList");
        }
    }
}
