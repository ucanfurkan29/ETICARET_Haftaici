using Microsoft.AspNetCore.Identity;

namespace ETICARET.WebUI.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            var username = configuration["Data:AdminUser:username"];
            var password = configuration["Data:AdminUser:password"];
            var email = configuration["Data:AdminUser:email"];
            var role = configuration["Data:AdminUser:role"];

            if (await userManager.FindByEmailAsync(email) == null) //bu maile sahip bir kullanıcı yoksa
            {
                await roleManager.CreateAsync(new IdentityRole(role)); //rolü oluştur

                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    FullName = "Furkan Ucan",
                    EmailConfirmed = true, //e-posta onaylı olarak kaydet
                };

                var result = await userManager.CreateAsync(user, password); //kullanıcıyı oluştur
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role); //kullanıcıyı role ekle
                }
                //Asenkron olmasının sebebi veritabanı işlemlerinin zaman alabilmesi ve uygulamanın donmaması için
            }

        }
    }
}
