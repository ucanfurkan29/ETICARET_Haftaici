using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface IRepository<T>
    {
        T GetById(int id); // Belirli bir ID'ye sahip varlığı getirir
        T GetOne(Expression<Func<T,bool>> filter = null); //belirtilen koşula uyan tek bir varlığı getirir
        //Expression filtreleme imkanı sağlar
        //Func ise bir temsilci türüdür ve bool türünde bir sonuç döndüren bir işlevi temsil eder
        //filter parametresi, GetOne metoduna bir koşul sağlamak için kullanılır

        List<T> GetAll(Expression<Func<T,bool>> filter = null); // Tüm varlıkları veya belirtilen koşula uyan varlıkları getirir

        void Create(T entity); // Yeni bir varlık oluşturur
        void Update(T entity); // Mevcut bir varlığı günceller
        void Delete(T entity); // Bir varlığı siler
    }
}

/*
 Bu yapı, veri erişim katmanında ortak CRUD (Create, Read, Update, Delete) işlemlerini tanımlamak için kullanılır.
tek bir yerden tüm varlık türleri için standart bir arayüz sağlar.
generic yapısı sayesinde, farklı sınıf türleri için aynı arayüzü kullanarak kod tekrarını azaltır ve bakımını kolaylaştırır.
 
 */
