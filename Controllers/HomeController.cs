using ArkCustomerManagement.Data;
using ArkCustomerManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ArkCustomerManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ArkCmsDbContext _context;

        public HomeController(ILogger<HomeController> logger, ArkCmsDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> CustomerList()
        {
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        [HttpGet]
        public IActionResult AddCustomer() 
        { 
            return View(new AddCustomer()); 
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(AddCustomer res)
        {
            if (ModelState.IsValid)
            {
                var newCustomer = new Customer
                {
                    Name = res.Name,
                    Address = res.Address,
                    TelephoneNumber = res.TelephoneNumber,
                    ContactPersonName = res.ContactPersonName,
                    ContactPersonEmail = res.ContactPersonEmail,
                    Vat = res.Vat
                };
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                return RedirectToAction("CustomerList");
            }

            return View(res);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var editCustomer = new EditCustomer
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Address = customer.Address,
                TelephoneNumber = customer.TelephoneNumber,
                ContactPersonName = customer.ContactPersonName,
                ContactPersonEmail = customer.ContactPersonEmail,
                Vat = customer.Vat
            };

            return View(editCustomer);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(int id, [bind("CustomerId,Name,Address,TelephoneNumber,ContactPersonName,ContactPersonEmail,Vat")] EditCustomer res)
        {
            if (id != res.CustomerId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.Name = res.Name;
                customer.Address = res.Address;
                customer.TelephoneNumber = res.TelephoneNumber;
                customer.ContactPersonName = res.ContactPersonName;
                customer.ContactPersonEmail = res.ContactPersonEmail;
                customer.Vat = res.Vat;

                _context.Update(customer);
                await _context.SaveChangesAsync();

                return RedirectToAction("CustomerList");
            }

            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("CustomerList");
        }
    }
}
