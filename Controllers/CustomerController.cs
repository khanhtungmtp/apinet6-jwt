using apinet6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apinet6.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [Authorize(Roles = "admin")]

    public class CustomerController : ControllerBase
    {
        private readonly Learn_DBContext _dBContext;

        public CustomerController(Learn_DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        [HttpGet]
        public IEnumerable<TblCustomer> Get()
        {
            return _dBContext.TblCustomer.ToList();
        }
    }
}