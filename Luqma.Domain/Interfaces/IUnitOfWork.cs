using Luqma.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luqma.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IGenericRepository<Order> Orders { get; }
        //IStockRepository Stocks { get; }
        //IGenericRepository<StockDetails> StockDetails { get; }
        //IProductRepository Products { get; }

        Task<bool> SaveChangesAsync();
    }
}
