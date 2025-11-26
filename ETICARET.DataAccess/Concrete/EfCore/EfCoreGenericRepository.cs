using ETICARET.DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreGenericRepository<T, TContext> : IRepository<T> where T : class where TContext : DbContext, new()
        //T:entity tipi, Tcontext:DbContext türünden bir context tipi, new():Parametresiz ctor a sahip olmalı, dbcontextten türemiş olmalı
    {

        //Yeni Nesne oluşturma
        //insert işlemi yapacak ama neye yapacağı belli değil generic olacak
        public void Create(T entity)
        {
            using (var context = new TContext())
            {
                context.Set<T>().Add(entity); //Dbset üzerinden erişip İlgili entity i ekliyoruz
                context.SaveChanges();
            }
        }

        //Nesne silme
        //virtual diyoruz çünkü bu metodu miras alan sınıflarda isteğe bağlı olarak ezebiliriz
        //örneğin bir entity silme işleminde farklı bir işlem yapmak istersek bu metodu override edebiliriz
        public virtual void Delete(T entity)
        {
            using (var context = new TContext())
            {
                context.Set<T>().Remove(entity); //Dbset üzerinden erişip İlgili entity i sil
                context.SaveChanges();
            }
        }

        //Nesne listeleme
        public virtual List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            using (var context = new TContext())
            {
                //filter null ise tüm verileri getir, değilse filtreye göre getir
                return filter == null
                    ? context.Set<T>().ToList()
                    : context.Set<T>().Where(filter).ToList();
                //eğer filtre null ise tüm verileri getir, değilse filtreye uyan nesneleri getir
            }
        }

        //ID'ye göre nesne getirme
        public T GetById(int id)
        {
            using (var context = new TContext())
            {
                return context.Set<T>().Find(id); //Dbset üzerinden erişip İlgili entity i id ile bul
                //set<T>() belirli bir türdeki varlıkların DbSet'ine erişim sağlar
            }
        }

        //Belirli bir koşula göre nesne getirme
        public T GetOne(Expression<Func<T, bool>> filter = null)
        {
            using (var context = new TContext())
            {
                return context.Set<T>().Where(filter).FirstOrDefault(); //filtreye uyan ilk nesneyi getir

            }
        }

        //Nesne güncelleme
        public virtual void Update(T entity)
        {
            using (var context = new TContext())
            {
                context.Entry(entity).State = EntityState.Modified; //ilgili entity nin durumunu güncellenmiş olarak işaretle
                //Entry metodu, belirli bir varlık örneğinin bağlamdaki durumunu alır veya ayarlar
                context.SaveChanges();
            }
        }
    }
}
