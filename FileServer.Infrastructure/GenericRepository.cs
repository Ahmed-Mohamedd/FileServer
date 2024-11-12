using FileServer.Core.Repositories_Interfaces;
using FileServer.Core.Specifications;
using FileServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly FileContext _fileContext;
        public GenericRepository(FileContext fileContext)
        {
            _fileContext=fileContext;
        }


        public async Task<IEnumerable<T>> GetAll()
            => await _fileContext.Set<T>().ToListAsync();
        
        public async Task Add(T entity)
            => await _fileContext.Set<T>().AddAsync(entity);
        public void Update(T entity)
            => _fileContext.Set<T>().Update(entity);

        public async Task Delete(ISpecification<T> spec)
            => await ApplySpecifications(spec).ExecuteDeleteAsync();


        public async Task<IEnumerable<T>> GetAllWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).ToListAsync();
        }

        public async Task<T?> GetByIdWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        public IQueryable<T> ApplySpecifications(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_fileContext.Set<T>(), spec);
        }

        public async Task SaveChangesAsync()
            => await _fileContext.SaveChangesAsync();

        public async Task<T> GetById(string id)
            => await _fileContext.Set<T>().FindAsync(id);

        public void DeleteWithoutSpec(T t)
            => _fileContext.Set<T>().Remove(t);
    }
}
