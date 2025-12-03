using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace ETICARET.WebUI.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            //nesneyi JSON stringine çevirip TempData'ya ekliyoruz
            //JsonConvert.SerializeObject: Nesneyi JSON formatına dönüştürür
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o; //geçici bir nesne
            tempData.TryGetValue(key, out o); //TempData'dan değeri almaya çalışıyoruz başarılı olursa o'ya atar

            //eğer o null değilse JSON stringini nesneye dönüştürüp döndürüyoruz
            //JsonConvert.DeserializeObject<T>: JSON formatındaki stringi nesneye dönüştürür
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
