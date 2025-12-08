using ETICARET.Business.Abstract;
using ETICARET.WebUI.EmailService;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ETICARET.WebUI.Controllers
{
    public class AccountController : Controller
    {
        //kullanıcı yönetimi işlemlerini yapabilmek için UserManager sınıfını kullanıyoruz.
        private UserManager<ApplicationUser> _userManager;
        //Oturum işlemlerini yönetmek için SignInManager sınıfını kullanıyoruz.
        private SignInManager<ApplicationUser> _signInManager;
        //Sepet işlemlerini yönetmek için ICartService arayüzünü kullanıyoruz.
        private ICartService _cartService;

        //constructor içinde servislerin ve yöneticilerin dependency injection ile alınması
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            //model doğrulama(required alanlar dolu mu?)
            if (!ModelState.IsValid)
            {
                return View(model); //eksik alan varsa tekrar kayıt sayfasına yönlendir
            }

            //yeni bir kullanıcı oluşturma
            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //kullanıcıya email onayı için token oluşturma
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //email onay için calback url oluşturma
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });

                string siteUrl = $"https://localhost:7217";

                string activeUrel = $"{siteUrl}{callbackUrl}";

                string body = $"<h2>Hesabı onaylanıyınız</h2> <br> <br> Hesabınızı onaylamak için lütfen <a href='{activeUrel}'>buraya tıklayın</a>.";

                MailHelper.SendEmail(body, user.Email, "ETICARET Hesap Onaylama");

                return RedirectToAction("Login", "Account");
            }
            //başarısızsa aynı sayfaya yönlendir
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //gerekli parametreler gelmediyse
            if (userId == null || token == null)
            {
                TempData.Put("message", new ResultModel
                {
                    Title = "Geçersiz token",
                    Message = "Hesap onay bilgileri yanlış",
                    Css = "danger"
                });
                return Redirect("~"); //anasayfaya yönlendir
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    _cartService.InitialCart(userId);
                    TempData.Put("message", new ResultModel
                    {
                        Title = "Hesap Onaylandı",
                        Message = "Hesabınız başarıyla onaylandı. Giriş yapabilirsiniz.",
                        Css = "success"
                    });
                    return RedirectToAction("Login", "Account");
                }
            }
            TempData.Put("message", new ResultModel
            {
                Title = "Hesap onaylanamadı",
                Message = "Hesabınız onaylanırken bir hata oluştu.",
                Css = "danger"
            });

            return Redirect("~"); //anasayfaya yönlendir
        }

        public IActionResult Login(string returnUrl = null) //returnUrl: kullanıcı giriş yaptıktan sonra yönlendirilecek url
        {
            return View(
                new LoginModel()
                {
                    ReturnUrl = returnUrl
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ModelState.Remove("ReturnUrl"); //returnUrl alanını model doğrulamasından çıkar
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel
                {
                    Title = "Hata",
                    Message = "Lütfen tüm alanları eksiksiz doldurun.",
                    Css = "danger"
                });

                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email); //email ile kullanıcıyı bul
            if (user is null)
            {
                ModelState.AddModelError("", "Bu email adresi ile kayıtlı bir kullanıcı bulunamadı");
                return View(model);
            }

            //kullanıcı bulunduysa şifre doğrulama
            //1.true: kullanıcıyı kalıcı olarak oturum açmış yap
            //2.true: başarısız giriş denemelerinde hesabı kilitle
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/"); //returnUrl varsa oraya yoksa anasayfaya yönlendir
            }
            if (result.IsLockedOut)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap kilitlendi",
                    Message = "Çok fazla başarısız giriş denemesi yaptınız. Hesabınız bir süreliğine kilitlendi.",
                    Css = "danger"
                });
                return View(model);
            }
            ModelState.AddModelError("", "Email veya şifre hatalı.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //oturumu kapat

            TempData.Put("message", new ResultModel()
            {
                Title = "Başarıyla çıkış yapıldı",
                Message = "Hesabınızdan başarıyla çıkış yaptınız.",
                Css = "success"
            });
            return Redirect("~/"); //anasayfaya yönlendir
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi unuttum",
                    Message = "Lütfen Email adresini boş bırakmayın",
                    Css = "danger"
                });
                return View();
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi unuttum",
                    Message = "bu email adresi ile kullanıcı bulunamadı",
                    Css = "danger"
                });
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            });
            string siteUrl = $"https://localhost:7217";
            string resetUrl = $"{siteUrl}{callbackUrl}";
            string body = $"<h2>Şifre sıfırlama talebi</h2> <br> <br> Şifrenizi sıfırlamak için lütfen <a href='{resetUrl}'>buraya tıklayın</a>.";
            MailHelper.SendEmail(body, email, "ETICARET Şifre Sıfırlama");
            TempData.Put("message", new ResultModel()
            {
                Title = "Şifremi unuttum",
                Message = "Şifre sıfırlama linki email adresinize gönderildi",
                Css = "success"
            });
            return RedirectToAction("Login");
        }

        public IActionResult ResetPassword(string token)
        {
            if (token ==null)
            {
                return RedirectToAction("Home", "Index");
            }
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Bu Email adresi ile kullanıcı bulunamadı.",
                    Css = "danger"
                });
                return RedirectToAction("Home", "Index");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                // Hata mesajıyla tekrar ResetPassword sayfasına dönülür
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Şifreniz uygun değildir.",
                    Css = "danger"
                });

                return View(model);
            }
        }

        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Bağlantı Hatası",
                    Message = "Kullanıcı bilgileri bulunamadı tekrar deneyin.",
                    Css = "danger"
                });
                return View();
            }
            var model = new AccountModel
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(AccountModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Giriş Bilgileri",
                    Message = "Bilgileriniz Hatalıdır",
                    Css = "danger"
                });
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User); //oturum açan kullanıcıyı al
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Bağlantı Hatası",
                    Message = "Kullanıcı bilgileri bulunamadı tekrar deneyin.",
                    Css = "danger"
                });
                return RedirectToAction("Login", "Account");
            }
            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            if (model.Email != user.Email)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token = code
                });
                string siteUrl = $"https://localhost:7217";
                string resetUrl = $"{siteUrl}{callbackUrl}";
                string body = $"<h2>Şifre sıfırlama talebi</h2> <br> <br> Şifrenizi sıfırlamak için lütfen <a href='{resetUrl}'>buraya tıklayın</a>.";
                MailHelper.SendEmail(body, model.Email, "ETICARET Şifre Sıfırlama");
                TempData.Put("message", new ResultModel()
                {
                    Title = "Email Değişikliği",
                    Message = "Email adresiniz değiştiği için yeni email adresinize şifre sıfırlama linki gönderildi.",
                    Css = "warning"
                });
                return RedirectToAction("Login", "Account");
            }
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Bilgileri Güncellendi",
                    Message = "Bilgileriniz başarıyla güncellenmiştir.",
                    Css = "success"
                });
                return RedirectToAction("Index", "Home");
            }
            TempData.Put("message", new ResultModel()
            {
                Title = "Hata",
                Message = "Bilgileriniz güncellenemedi. Lütfen tekrar deneyin.",
                Css = "danger"
            });
            return View(model);

        }
    }
}
