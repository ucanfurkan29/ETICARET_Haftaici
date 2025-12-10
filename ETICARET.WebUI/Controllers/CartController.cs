using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETICARET.WebUI.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;
        private IProductService _productService;
        private IOrderService _orderService;

        private UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, IProductService productService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            return View(
                new CartModel() 
                { 
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        Name = i.Product.Name,
                        ProductId = i.Product.Id,
                        Quantity = i.Quantity,
                        Price = i.Product.Price,
                        ImageUrl = i.Product.Images[0].ImageUrl
                    }).ToList()
                }
            );
        }

        public IActionResult AddToCart(int productId, int quantity)
        {
            _cartService.AddToCart(_userManager.GetUserId(User), productId, quantity);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteFromCart(int productId)
        {
            _cartService.DeleteFromCart(_userManager.GetUserId(User), productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            OrderModel orderModel = new OrderModel();

            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    Name = i.Product.Name,
                    ProductId = i.Product.Id,
                    Quantity = i.Quantity,
                    Price = i.Product.Price,
                    ImageUrl = i.Product.Images[0].ImageUrl
                }).ToList()
            };
            return View(orderModel);
        }
        [HttpPost]
        public IActionResult Checkout(OrderModel model, string paymentMethod)
        {
            ModelState.Remove("CartModel");

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var cart = _cartService.GetCartByUserId(userId);

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        Name = i.Product.Name,
                        ProductId = i.Product.Id,
                        Quantity = i.Quantity,
                        Price = i.Product.Price,
                        ImageUrl = i.Product.Images[0].ImageUrl
                    }).ToList()
                };
                if (paymentMethod == "credit")
                {
                    var payment = PaymentProccess(model);
                    if (payment.Result.Status == "success")
                    {
                        SaveOrder(model,payment,userId);
                        ClearCart(cart.Id.ToString());
                        TempData.Put("message", new ResultModel()
                        {
                            Title = "Order Completed",
                            Message = "Your order has been completed successfully",
                            Css = "success"
                        });
                    }
                    else
                    {
                        TempData.Put("message", new ResultModel()
                        {
                            Title = "Order Uncompleted",
                            Message = "Your order could not be completed",
                            Css = "danger"
                        });
                    }
                }
                else
                {
                    SaveOrder(model, userId);
                    ClearCart(cart.Id.ToString());
                    TempData.Put("message", new ResultModel()
                    {
                        Title = "Order Completed",
                        Message = "Your order has been completed successfully",
                        Css = "success"
                    });
                }
            }
            return View(model);
        }

        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = _orderService.GetOrders(userId);
            var orderListModel = new List<OrderListModel>();

            OrderListModel orderModel;
            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.OrderState = order.OrderState;
                orderModel.Address = order.Address;
                orderModel.PaymentType = order.PaymentType;
                orderModel.OrderNote= order.OrderNote;
                orderModel.City= order.City;
                orderModel.Email= order.Email;
                orderModel.FirstName= order.FirstName;
                orderModel.LastName= order.LastName;
                orderModel.Phone= order.Phone;

                orderModel.OrderItems = order.OrderItems.Select(i => new OrderItemModel()
                {
                    OrderItemId = i.Id,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Name = i.Product.Name,
                    ImageUrl = i.Product.Images[0].ImageUrl
                }).ToList();

                orderListModel.Add(orderModel);
            }
            return View(orderListModel);

        }



        private void ClearCart(string id)
        {
            _cartService.ClearCart(id);
        }

        private void SaveOrder(OrderModel model, string userId)
        {
            Order order = new Order()
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = EnumOrderState.completed,
                PaymentType = EnumPaymentType.Eft,
                PaymentToken = Guid.NewGuid().ToString(),
                ConversionId = Guid.NewGuid().ToString(),
                PaymentId = Guid.NewGuid().ToString(),
                OrderNote = model.OrderNote,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                Phone = model.Phone,
                Email = model.Email,
                UserId = userId
            };
            foreach (var cartItem in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = cartItem.Price,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }

        private void SaveOrder(OrderModel model,Task<Payment> payment, string userId)
        {
            Order order = new Order()
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = EnumOrderState.completed,
                PaymentType = EnumPaymentType.Eft,
                PaymentToken = Guid.NewGuid().ToString(),
                ConversionId = Guid.NewGuid().ToString(),
                PaymentId = Guid.NewGuid().ToString(),
                OrderNote = model.OrderNote,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                Phone = model.Phone,
                Email = model.Email,
                UserId = userId
            };
            foreach (var cartItem in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = cartItem.Price,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }

        private async Task<Payment> PaymentProccess(OrderModel model)
        {
            // Iyzipay sandbox ortamı için gerekli kimlik bilgileri
            Options options = new Options()
            {
                BaseUrl = "https://sandbox-api.iyzipay.com",
                ApiKey = "sandbox-cNnJEaoyNt0sCREL4nOq8PajTLQwWeXz",
                SecretKey = "sandbox-cmJxJfaGlVarqNV3c5ZQcMTwVNh8qswx"
            };

            // Kullanıcının IP adresini almak için
            // IP adresi icanhazip.com'dan çekiliyor, gereksiz satır sonu karakterleri temizleniyor
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            var externalIp = IPAddress.Parse(externalIpString);

            // Ödeme isteği oluşturuluyor
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString(); // Türkçe işlem
            request.ConversationId = Guid.NewGuid().ToString(); // İşleme ait benzersiz ID
            request.Price = model.CartModel.TotalPrice().ToString().Split(',')[0]; // Toplam tutar (kuruş kısım ayırma)
            request.PaidPrice = model.CartModel.TotalPrice().ToString().Split(',')[0]; // Ödenen tutar
            request.Currency = Currency.TRY.ToString(); // Para birimi Türk Lirası
            request.Installment = 1; // Taksit sayısı
            request.BasketId = model.CartModel.CartId.ToString(); // Sepet Id
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString(); // Ürün grubu
            request.PaymentChannel = PaymentChannel.WEB.ToString(); // Web kanalı üzerinden ödeme

            // Kredi kartı bilgileri
            PaymentCard paymentCard = new PaymentCard()
            {
                CardHolderName = model.CardName,
                CardNumber = model.CardNumber,
                ExpireYear = model.ExprationYear,
                ExpireMonth = model.ExprationMonth,
                Cvc = model.CVV,
                RegisterCard = 0 // Kayıtlı kart olarak eklenmesin
            };

            // Request'e kredi kartı bilgisi ekleniyor
            request.PaymentCard = paymentCard;

            // Ödeme yapan (Buyer) bilgileri
            Buyer buyer = new Buyer()
            {
                Id = _userManager.GetUserId(User),
                Name = model.FirstName,
                Surname = model.LastName,
                GsmNumber = model.Phone,
                Email = model.Email,
                IdentityNumber = "11111111111", // Örnek kimlik numarası
                RegistrationAddress = model.Address,
                Ip = externalIp.ToString(),
                City = model.City,
                Country = "TURKEY",
                ZipCode = "34000"
            };

            request.Buyer = buyer;

            // Fatura adresi ve gönderim adresi aynı kabul ediliyor
            Address address = new Address()
            {
                ContactName = model.FirstName + " " + model.LastName,
                City = model.City,
                Country = "Turkey",
                Description = model.Address,
                ZipCode = "34000"
            };

            request.BillingAddress = address;
            request.ShippingAddress = address;

            // Sepetteki ürünleri Iyzipay basket item listesine dönüştürür
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var cartItem in model.CartModel.CartItems)
            {
                basketItem = new BasketItem()
                {
                    Id = cartItem.ProductId.ToString(),
                    Name = cartItem.Name,
                    Category1 = _productService.GetProductDetail(cartItem.ProductId).ProductCategories.FirstOrDefault().CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (cartItem.Price * cartItem.Quantity).ToString().Split(",")[0] // Ürün fiyatı x adet
                };

                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            // Iyzipay Payment yaratılır (async)
            Payment payment = await Payment.Create(request, options);

            // Payment nesnesi döndürülür (içinde Status, PaymentId, ConversationId, vs. bilgileri var)
            return payment;
        }





    }
}
