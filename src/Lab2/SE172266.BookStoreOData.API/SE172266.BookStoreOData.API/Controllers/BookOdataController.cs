using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SE172266.BookStoreOData.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Extensions.Logging;
using SE172266.BookStoreOData.API.Model;

namespace SE172266.BookStoreOData.API.Controllers
{
    public class BooksController : ODataController
    {
        private readonly BookStoreODataDBContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(BookStoreODataDBContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /odata/Books
        [EnableQuery]
        public IQueryable<Book> Get()
        {
            _logger.LogInformation("Fetching all books");
            return _context.Books;
        }

        // GET /odata/Books({key})
        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            _logger.LogInformation($"Fetching book with ID {key}");
            var book = await _context.Books.FindAsync(key);
            if (book == null)
            {
                _logger.LogWarning($"Book with ID {key} not found");
                return NotFound();
            }
            return Ok(book);
        }

        // POST /odata/Books
        public async Task<IActionResult> Post([FromBody] BookCreateModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = new Book
            {
                Isbn = createModel.Isbn,
                Title = createModel.Title,
                Author = createModel.Author,
                Price = createModel.Price,
                LocationId = createModel.LocationId,
                PressId = createModel.PressId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Created(book);
        }

        // PUT /odata/Books({key})
        public async Task<IActionResult> Put(int key, [FromBody] BookUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = await _context.Books.FindAsync(key);
            if (existingBook == null)
            {
                return NotFound();
            }

            // Update properties
            existingBook.Isbn = updateModel.Isbn;
            existingBook.Title = updateModel.Title;
            existingBook.Author = updateModel.Author;
            existingBook.Price = updateModel.Price;
            existingBook.LocationId = updateModel.LocationId;
            existingBook.PressId = updateModel.PressId;

            _context.Entry(existingBook).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Updated(existingBook);
        }

        // DELETE /odata/Books({key})
        public async Task<IActionResult> Delete(int key)
        {
            var book = await _context.Books.FindAsync(key);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
