using Luqma.Domain.Interfaces;
using Luqma.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luqma.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LuqmaContext _context;

        //public IGenericRepository<Order> Orders { get; }
        //public IStockRepository Stocks { get; }
        //public IGenericRepository<StockDetails> StockDetails { get; }
        //public IProductRepository Products { get; }

        public UnitOfWork(
            LuqmaContext context
            //IGenericRepository<Order> orderRepository,
            //IStockRepository stockRepository,
            //IGenericRepository<StockDetails> stockDetailsRepository,
            //IProductRepository productRepository
            )
        {
            _context = context;
            //Orders = orderRepository;
            //Stocks = stockRepository;
            //StockDetails = stockDetailsRepository;
            //Products = productRepository;
        }

        public async Task<bool> SaveChangesAsync()
        {
            var rowsAffacted =  await _context.SaveChangesAsync();
            return rowsAffacted > 0;
        }
        public void Dispose()
            => _context.Dispose();
    }

}
