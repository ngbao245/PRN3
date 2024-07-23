using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SE172266.BookStoreOData.API.Repository;

namespace SE172266.BookStoreOData.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PressesController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public PressesController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_unitOfWork.PressRepository.Get());
        }
    }
}
