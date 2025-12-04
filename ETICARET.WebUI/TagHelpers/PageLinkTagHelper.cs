using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ETICARET.WebUI.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")] // div etiketine page-model attribute'u eklenirse bu TagHelper çalışır
    public class PageLinkTagHelper : TagHelper
    {
        public PageInfo PageModel { get; set; }


        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div"; // Hedeflenen etiket div

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<ul class='pagination'>"); // Bootstrap pagination sınıfı başlangıcı
            for (int i = 1; i <= PageModel.TotalPages(); i++)
            {
                //Her sayfa için list item oluştur
                //mevcut sayfa ise active class'ı ekle
                stringBuilder.AppendFormat("<li class='page-item {0}'>", i == PageModel.CurrentPage ? "active" : "");

                if (string.IsNullOrEmpty(PageModel.CurrentCategory))
                {
                    stringBuilder.AppendFormat("<a class='page-link' href='/products?page={0}'>{0}</a>", i);
                }
                else
                {
                    stringBuilder.AppendFormat("<a class='page-link' href='/products/{0}?page={1}'>{1}</a>"
                        ,PageModel.CurrentCategory ,i);

                }
                stringBuilder.Append("</li>");
            }
            stringBuilder.Append("</ul>"); // Bootstrap pagination sınıfı sonu

            output.Content.SetHtmlContent(stringBuilder.ToString()); // Oluşturulan HTML içeriğini çıktı olarak ayarla

            base.Process(context, output);
        }

    }
}
