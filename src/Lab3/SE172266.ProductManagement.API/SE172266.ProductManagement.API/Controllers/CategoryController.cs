using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SE172266.ProductManagement.API.Model.Category;
using SE172266.ProductManagement.Repo.Entities;
using SE172266.ProductManagement.Repo.Repository;
using SE172266.ProductManagement.Repo.Model;
using System;
using System.Linq;

namespace SE172266.ProductManagement.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public CategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var responseCategories = _unitOfWork.CategoryRepository.Get();
            return Ok(responseCategories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = $"Category with id {id} not found." } }
                });
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateCategory([FromBody] CategoryModel requestCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new Error { Message = e.ErrorMessage }).ToList()
                });
            }

            var categoryEntity = new Category
            {
                CategoryName = requestCategoryModel.CategoryName
            };
            _unitOfWork.CategoryRepository.Insert(categoryEntity);
            _unitOfWork.SaveChange();
            return Ok(new { Message = "Category created successfully", CategoryId = categoryEntity.CategoryId });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryModel requestCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new Error { Message = e.ErrorMessage }).ToList()
                });
            }

            var existedCategoryEntity = _unitOfWork.CategoryRepository.GetById(id);
            if (existedCategoryEntity == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = $"Category with id {id} not found." } }
                });
            }

            existedCategoryEntity.CategoryName = requestCategoryModel.CategoryName;
            _unitOfWork.CategoryRepository.Update(existedCategoryEntity);
            _unitOfWork.SaveChange();
            return Ok(new { Message = "Category updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var existedCategoryEntity = _unitOfWork.CategoryRepository.GetById(id);
                if (existedCategoryEntity == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Errors = new List<Error> { new Error { Message = $"Category with id {id} not found." } }
                    });
                }
                _unitOfWork.CategoryRepository.Delete(id);
                _unitOfWork.SaveChange();
                return Ok(new { Message = "Category deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = $"Internal server error: {ex.Message}" } }
                });
            }
        }
    }
}
