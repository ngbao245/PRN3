using SE172266.ProductManagement.Repo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE172266.ProductManagement.Repo.Repositories
{
    public class UnitOfWork
    {
        private DBContext _context;
        private GenericRepository<Category> _category;
        private GenericRepository<Product> _product;


        public UnitOfWork(DBContext context)
        {
            _context = context;
        }

        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (_category == null)
                {
                    this._category = new GenericRepository<Category>(_context);
                }
                return _category;
            }

        }
        public GenericRepository<Product> ProductRepository
        {
            get
            {
                if (_product == null)
                {
                    this._product = new GenericRepository<Product>(_context);
                }
                return _product;
            }

        }


        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
