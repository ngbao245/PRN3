using Microsoft.AspNetCore.Mvc;
using SE172266.ProductManagement.Repo.Model;
using SE172266.BookStoreOData.API.Repository;
using SE172266.ProductManagement.Repo.Entities;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SE172266.BookStoreOData.API.Model;

namespace SE172266.BookStoreOData.API.Controllers
{
    [Route("odata/Books")]
    [ApiController]
    public class BooksController : ODataController
    {
        private readonly UnitOfWork _unitOfWork;

        public BooksController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [EnableQuery(PageSize = 50)]
        public IActionResult Get()
        {
            var books = _unitOfWork.BookRepository.Get();
            if (books == null || !books.Any())
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Errors = new List<Error> { new Error { Message = "No books found." } }
                });
            }

            return Ok(books);
        }

        [EnableQuery]
        public IActionResult Get([FromODataUri] int key)
        {
            var book = _unitOfWork.BookRepository.GetById(key);
            if (book == null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Errors = new List<Error> { new Error { Message = "Book not found." } }
                });
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Errors = new List<Error> { new Error { Message = "Invalid book data." } }
                });
            }

            _unitOfWork.BookRepository.Insert(book);
            _unitOfWork.SaveChange();
            return Created(book);
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int key)
        {
            var book = _unitOfWork.BookRepository.GetById(key);
            if (book == null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Errors = new List<Error> { new Error { Message = "Book not found." } }
                });
            }

            _unitOfWork.BookRepository.Delete(book.Id);
            _unitOfWork.SaveChange();
            return Ok();
        }

        [HttpPut("{key}")]
        public IActionResult Update(int key, [FromBody] BookUpdateModel book)
        {
            var existingBook = _unitOfWork.BookRepository.GetById(key);
            if (existingBook == null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Errors = new List<Error> { new Error { Message = "Book not found." } }
                });
            }

            // Validate and update the book
            if (book == null)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Errors = new List<Error> { new Error { Message = "Invalid book update data." } }
                });
            }

            existingBook.Author = book.Author;
            existingBook.ISBN = book.Isbn;
            existingBook.Price = book.Price;
            existingBook.Location = book.Location;
            existingBook.PressId = book.PressId;
            existingBook.Title = book.Title;

            _unitOfWork.BookRepository.Update(existingBook);
            _unitOfWork.SaveChange();
            return Ok();
        }
    }
}
