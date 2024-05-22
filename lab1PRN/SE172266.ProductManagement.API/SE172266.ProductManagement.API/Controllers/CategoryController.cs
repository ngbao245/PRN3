using Microsoft.AspNetCore.Mvc;
using SE172266.ProductManagement.API.Model.CategoryModel;
using SE172266.ProductManagement.Repo.Entities;
using SE172266.ProductManagement.Repo.Repositories;

namespace SE172266.ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("id")]
        public IActionResult GetById(int id)
        {
            var responseCategories = _unitOfWork.CategoryRepository.GetByID(id);
            return Ok(responseCategories);
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel requestCategoryModel)
        {
            var categoryEntity = new Category
            {
                CategoryName = requestCategoryModel.CategoryName
            };
            _unitOfWork.CategoryRepository.Insert(categoryEntity);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPut("id")]
        public IActionResult UpdateCategory(int id, CategoryModel requestCategoryModel)
        {
            var existedCategoryEntity = _unitOfWork.CategoryRepository.GetByID(id);
            if (existedCategoryEntity != null)
            {
                existedCategoryEntity.CategoryName = requestCategoryModel.CategoryName;
            }
            _unitOfWork.CategoryRepository.Update(existedCategoryEntity);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpDelete("id")]
        public IActionResult DeleteCategory(int id)
        {
            var existedCategoryEntity = _unitOfWork.CategoryRepository.GetByID(id);
            _unitOfWork.CategoryRepository.Delete(existedCategoryEntity);
            _unitOfWork.Save();
            return Ok();
        }

    }
}
