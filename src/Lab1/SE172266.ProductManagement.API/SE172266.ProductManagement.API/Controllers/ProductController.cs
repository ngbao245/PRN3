using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE172266.ProductManagement.API.Model.ProductModel;
using SE172266.ProductManagement.Repo.Entities;
using SE172266.ProductManagement.Repo.Repositories;
using System.Linq.Expressions;

namespace SE172266.ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ProductController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// SortBy (ProductId = 1, ProductName = 2, CategoryId = 3, UnitsInStock = 4, UnitPrice = 5)
        /// 
        /// SortType (Ascending = 1, Descending = 2)
        /// </summary>
        /// <param name="requestSearchProductModel"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public IActionResult SearchProduct([FromQuery] SearchProductModel requestSearchProductModel)
        {
            var sortBy = requestSearchProductModel.SortContent?.sortProductBy.ToString();
            var sortType = requestSearchProductModel.SortContent?.sortProductType?.ToString();

            Expression<Func<Product, bool>> filter = x =>
                (string.IsNullOrEmpty(requestSearchProductModel.ProductName) || x.ProductName.Contains(requestSearchProductModel.ProductName)) &&
                (!requestSearchProductModel.CategoryId.HasValue || x.CategoryId == requestSearchProductModel.CategoryId) &&
                (x.UnitPrice >= requestSearchProductModel.FromUnitPrice &&
                 (!requestSearchProductModel.ToUnitPrice.HasValue || x.UnitPrice <= requestSearchProductModel.ToUnitPrice.Value));

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null;

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortType == SortProductTypeEnum.Ascending.ToString())
                {
                    orderBy = query => query.OrderBy(p => EF.Property<object>(p, sortBy));
                }
                else if (sortType == SortProductTypeEnum.Descending.ToString())
                {
                    orderBy = query => query.OrderByDescending(p => EF.Property<object>(p, sortBy));
                }
            }

            var responseProducts = _unitOfWork.ProductRepository.Get(
                filter,
                orderBy,
                includeProperties: "",
                pageIndex: requestSearchProductModel.pageIndex,
                pageSize: requestSearchProductModel.pageSize
            );

            return Ok(responseProducts);
        }

        [HttpPost]
        public IActionResult CreateProduct(CreateProductModel requestCreateProductModel)
        {
            var productEntity = new Product
            {
                CategoryId = requestCreateProductModel.CategoryId,
                ProductName = requestCreateProductModel.ProductName,
                UnitPrice = requestCreateProductModel.UnitPrice,
                UnitsInStock = requestCreateProductModel.UnitsInStock,
            };
            _unitOfWork.ProductRepository.Insert(productEntity);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPut("id")]
        public IActionResult UpdateProduct(int id, CreateProductModel requestCreateProductModel)
        {
            var existedProductEntity = _unitOfWork.ProductRepository.GetByID(id);
            if (existedProductEntity != null)
            {
                existedProductEntity.CategoryId = requestCreateProductModel.CategoryId;
                existedProductEntity.ProductName = requestCreateProductModel.ProductName;
                existedProductEntity.UnitPrice = requestCreateProductModel.UnitPrice;
                existedProductEntity.UnitsInStock = requestCreateProductModel.UnitsInStock;
            }
            _unitOfWork.ProductRepository.Update(existedProductEntity);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpDelete("id")]
        public IActionResult DeleteProduct(int id)
        {
            var existedCategoryEntity = _unitOfWork.ProductRepository.GetByID(id);
            _unitOfWork.ProductRepository.Delete(existedCategoryEntity);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
