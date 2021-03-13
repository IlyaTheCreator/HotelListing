using HotelListing.Data;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        /* The expression data type allows us to pass lambda expression
           Then we use that expression as a parameter for FirstOrDefaultAsync
           to tell it by what we want to get the entity */
        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            // Get all the records that are in that table
            IQueryable<T> query = _db;

            // Here we attach the result of each foreign key if we want to
            if (includes is not null)
            {
                foreach (var includeProperty in includes)   
                {
                    query = query.Include(includeProperty);
                }
            }

            /* Because there's no need for the EntityFramework to prepare each 
               included property for a potential update, we disable that
               because we want to spend as little resources as possible (AsNoTracking) */
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            // Filter the query first if there was any expression
            if (expression is not null)
            {
                query = query.Where(expression);
            }

            if (includes is not null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            // Conditional ordering
            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            // Here we say to the database: pay attention to this and check
            // if you have it already up-to-date (memory and stuff)
            _db.Attach(entity);

            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
