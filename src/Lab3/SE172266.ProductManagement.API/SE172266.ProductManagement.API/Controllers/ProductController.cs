using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE172266.ProductManagement.API.Model.ProductModel;
using SE172266.ProductManagement.Repo.Entities;
using SE172266.ProductManagement.Repo.Repository;
using SE172266.ProductManagement.API.Enum;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using SE172266.ProductManagement.Repo.Model;
using static SE172266.ProductManagement.API.Enum.Sort;

namespace SE172266.ProductManagement.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ProductController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = _unitOfWork.ProductRepository.GetById(id);
                if (product == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Errors = new List<Error> { new Error { Message = $"Product with id {id} not found." } }
                    });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = $"Internal server error: {ex.Message}" } }
                });
            }
        }


        /// <summary>
        /// SortBy (ProductId = 1, ProductName = 2, CategoryId = 3, UnitsInStock = 4, UnitPrice = 5)
        /// 
        /// SortType (Ascending = 1, Descending = 2)
        /// </summary>
        /// <param name="requestSearchProductModel"></param>
        /// <returns></returns>
        [HttpGet]
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
        [Authorize(Roles = "Admin")]
        public IActionResult CreateProduct([FromBody] CreateProductModel requestCreateProductModel)
        {
            // Check if categoryId exists
            var category = _unitOfWork.CategoryRepository.GetById(requestCreateProductModel.CategoryId);
            if (category == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = "Invalid category ID" } }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new Error { Message = e.ErrorMessage }).ToList()
                });
            }

            var productEntity = new Product
            {
                CategoryId = requestCreateProductModel.CategoryId,
                ProductName = requestCreateProductModel.ProductName,
                UnitPrice = requestCreateProductModel.UnitPrice,
                UnitsInStock = requestCreateProductModel.UnitsInStock,
            };

            _unitOfWork.ProductRepository.Insert(productEntity);
            _unitOfWork.SaveChange();
            return CreatedAtAction(nameof(GetProductById), new { id = productEntity.ProductId }, new { Message = "Product created successfully", Product = productEntity });
            //return new ObjectResult(productEntity) { StatusCode = 201 };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProduct(int id, [FromBody] CreateProductModel requestCreateProductModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new Error { Message = e.ErrorMessage }).ToList()
                });
            }

            var existedProductEntity = _unitOfWork.ProductRepository.GetById(id);
            if (existedProductEntity == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = $"Product with id {id} not found." } }
                });
            }

            //existedProductEntity.CategoryId = requestCreateProductModel.CategoryId;
            //existedProductEntity.ProductName = requestCreateProductModel.ProductName;
            //existedProductEntity.UnitPrice = requestCreateProductModel.UnitPrice;
            //existedProductEntity.UnitsInStock = requestCreateProductModel.UnitsInStock;

            // Update fields only if they are provided in the request
            if (requestCreateProductModel.CategoryId != default)
            {
                existedProductEntity.CategoryId = requestCreateProductModel.CategoryId;
            }

            if (!string.IsNullOrEmpty(requestCreateProductModel.ProductName))
            {
                existedProductEntity.ProductName = requestCreateProductModel.ProductName;
            }

            if (requestCreateProductModel.UnitPrice != default)
            {
                existedProductEntity.UnitPrice = requestCreateProductModel.UnitPrice;
            }

            if (requestCreateProductModel.UnitsInStock != default)
            {
                existedProductEntity.UnitsInStock = requestCreateProductModel.UnitsInStock;
            }

            _unitOfWork.ProductRepository.Update(existedProductEntity);
            _unitOfWork.SaveChange();
            return Ok(new { Message = "Product updated successfully", Product = existedProductEntity });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var existedProductEntity = _unitOfWork.ProductRepository.GetById(id);
                if (existedProductEntity == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Errors = new List<Error> { new Error { Message = $"Product with id {id} not found." } }
                    });
                }

                _unitOfWork.ProductRepository.Delete(id);
                _unitOfWork.SaveChange();
                return Ok(new { Message = "Product deleted successfully" });
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
