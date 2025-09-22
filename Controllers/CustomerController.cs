using ArkCustomerManagement.Data;
using ArkCustomerManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArkCustomerManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ArkCmsDbContext _context;

        public CustomerController(ArkCmsDbContext context)
        {
            _context = context;
        }

        public IActionResult ManageCustomers()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new Customer());
        }
    }
}
