using System.Net;
using System.Net.Mail;

namespace ETICARET.WebUI.EmailService
{
    public static class MailHelper
    {
        public static bool SendEmail(string body, string to, string subject, bool isHtml = true)
        {
            return SendEmail(body, new List<string> { to }, subject, isHtml);
        }

        private static bool SendEmail(string body, List<string> to, string subject, bool isHtml)
        {
            bool result = false; // Mail gönderme işleminin başarılı olup olmadığını tutan değişken
            try
            {
                var message = new MailMessage(); // Yeni bir MailMessage nesnesi oluşturuluyor
                message.From = new MailAddress("furkanucanuby@gmail.com"); // Gönderen e-posta adresi belirleniyor
                to.ForEach(x =>
                {
                    message.To.Add(new MailAddress(x)); // Alıcı e-posta adresleri ekleniyor
                });

                message.Subject = subject; // E-posta konusu belirleniyor
                message.Body = body; // E-posta içeriği belirleniyor
                message.IsBodyHtml = isHtml; // E-posta içeriğinin HTML formatında olup olmadığı belirleniyor

                using (var smtp = new SmtpClient("smtp.gmail.com", 587)) // SMTP istemcisi oluşturuluyor
                {
                    smtp.EnableSsl = true; // SSL kullanımı etkinleştiriliyor

                    smtp.Credentials = new NetworkCredential(
                        "furkanucanuby@gmail.com",
                        "xlfa sqxf zarj oyfb" // Uygulama şifresi kullanılıyor
                    );

                    smtp.UseDefaultCredentials = false; // Varsayılan kimlik bilgileri kullanılmıyor

                    smtp.Send(message); // E-posta gönderiliyor
                    result = true; // E-posta gönderme işlemi başarılı ise result true olarak ayarlanıyor
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = false; // Hata durumunda result false olarak ayarlanıyor
            }

            return result; // E-posta gönderme işleminin sonucu döndürülüyor
        }
    }
}
