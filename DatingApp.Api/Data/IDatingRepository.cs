using System.Threading.Tasks;
using System.Collections.Generic;
using DatingApp.Api.MOdels;

namespace DatingApp.Api.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class ;
         void Delete<T>(T entity) where T: class ;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int Id);
        Task<Photo> GetPhoto(int Id);
        Task<Photo> GetMainPhotoForUser(int id);
    }
}