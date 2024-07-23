
using SE172266.ProductManagement.Repo.Entities;

namespace SE172266.BookStoreOData.API.Repository
{
    public class UnitOfWork : IDisposable
    {
        private BookStoreODataDBContext _context;
        private GenericRepository<Book> _book;
        private GenericRepository<Press> _press;

        private bool disposed = false;

        public UnitOfWork(BookStoreODataDBContext context)
        {
            _context = context;
        }

        public GenericRepository<Book> BookRepository
        {
            get
            {
                if (_book == null)
                {
                    _book = new GenericRepository<Book>(_context);
                }
                return _book;
            }
        }

        public GenericRepository<Press> PressRepository
        {
            get
            {
                if (this._press == null)
                {
                    this._press = new GenericRepository<Press>(_context);
                }
                return _press;
            }
        }

        public void SaveChange()
        {
            _context.SaveChanges();
        }

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
