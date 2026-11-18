using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using Luqma.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luqma.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly LuqmaContext _dbContext;

        public GenericRepository(LuqmaContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAllIgnoreFilters()
        {
            return _dbContext.Set<TEntity>().IgnoreQueryFilters();
        }

        public async Task<TEntity> GetByIDAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(a => a.ID == id);
        }

        public async Task<TEntity> GetByIDIgnoreFiltersAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().IgnoreQueryFilters().SingleOrDefaultAsync(a => a.ID == id);
        }
        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Archived(TEntity entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbContext.Set<TEntity>().Attach(entity);
            }

            entity.IsArchived = true;
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void UnArchived(TEntity entity)
        {
            entity.ArchivedById = null;
            entity.ArchivedDateTime = null;
            entity.ArchivedByName = null;
            entity.IsArchived = false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}
