using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICommentService
    {
        Comment GetById(int id); // ID'ye göre yorum getirir
        void Create(Comment entity); // Yeni yorum oluşturur
        void Update(Comment entity); // Mevcut yorumu günceller
        void Delete(Comment entity); // Yorumu siler
    }
}
