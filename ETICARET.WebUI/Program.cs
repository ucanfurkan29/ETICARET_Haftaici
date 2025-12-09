using ETICARET.Business.Abstract;
using ETICARET.Business.Concrete;
using ETICARET.DataAccess.Abstract;
using ETICARET.DataAccess.Concrete.EfCore;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); //razor pages ekledik

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))
);

//Identity servislerini ekledik
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();

var userManager = builder.Services.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();//kullanýcý yöneticisi
var roleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();//rol yöneticisi

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireNonAlphanumeric = true; //en az bir özel karakter
    options.Password.RequireLowercase = true; //en az bir küçük harf
    options.Password.RequireUppercase = true; //en az bir büyük harf
    options.Password.RequireDigit = true; //en az bir rakam
    options.Password.RequiredLength = 6; //minimum 6 karakter

    //Hesap kilitleme ayarlarý
    options.Lockout.MaxFailedAccessAttempts = 5; //5 baþarýsýz giriþte kilitle
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //5 dakika kilitli kalýr
    options.Lockout.AllowedForNewUsers = true; //yeni kullanýcýlar için kilitleme aktif

    //Kullanýcý ayarlarý
    options.User.RequireUniqueEmail = true; //her kullanýcý için benzersiz e-posta
    options.SignIn.RequireConfirmedEmail = true; //e-posta onayý zorunlu 
    options.SignIn.RequireConfirmedPhoneNumber = false; //telefon onayý zorunlu deðil
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login"; //giriþ sayfasý
    options.LogoutPath = "/account/logout"; //çýkýþ sayfasý
    options.AccessDeniedPath = "/account/accessdenied"; //eriþim reddedildi sayfasý
    options.SlidingExpiration = true; //oturum kaydýrma
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //oturum süresi 60 dakika
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true, //sadece HTTP eriþimi
        Name = "ETICARET.Security.Cookie", //çerez adý
        SameSite = SameSiteMode.Strict, //ayný site kýsýtlamasý
        //farklý sitelerden gelen isteklerde çerezin gönderilmemesi
    };
});


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductDal,EfCoreProductDal>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<ICategoryDal, EfCoreCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICommentDal, EfCoreCommentDal>();
builder.Services.AddScoped<ICommentService, CommentManager>();
builder.Services.AddScoped<ICartDal, EfCoreCartDal>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IOrderDal, EfCoreOrderDal>();
builder.Services.AddScoped<IOrderService, OrderManager>();

builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

SeedDatabase.Seed();

app.UseStaticFiles();
app.CustomStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "CategoryEdit",
        pattern: "admin/categories/{id?}",
        defaults: new { controller = "Admin", action = "EditCategory" }
        );

    endpoints.MapControllerRoute(
        name: "adminProducts",
        pattern: "admin/products/{id?}",
        defaults: new { controller = "Admin", action = "EditProduct" }
        );

});


SeedIdentity.Seed(userManager, roleManager, app.Configuration).Wait();
app.Run();
