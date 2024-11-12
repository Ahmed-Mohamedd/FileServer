using FileServer.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Repositories_Interfaces
{
    public interface IGenericRepository<T> where T : class
    {

        Task<IEnumerable<T>> GetAllWithSpec(ISpecification<T> spec);
        Task<T> GetByIdWithSpec(ISpecification<T> spec);

        Task Add(T t);
        void Update(T t);
        Task Delete(ISpecification<T> spec);
        void DeleteWithoutSpec(T t);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task SaveChangesAsync();

    }
}
