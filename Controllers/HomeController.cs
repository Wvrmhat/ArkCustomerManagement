using ArkCustomerManagement.Data;
using ArkCustomerManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

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
        public async Task<IActionResult> CustomerList(string sort, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            ViewData["NameSortParm"] = sort == "Name" ? "Name_desc" : "Name";
            ViewData["AddressSortParm"] = sort == "Address" ? "Address_desc" : "Address";
            ViewData["TelephoneSortParm"] = sort == "Telephone" ? "Telephone_desc" : "Telephone";
            ViewData["ContactPersonNameSortParm"] = sort == "ContactPersonName" ? "ContactPersonName_desc" : "ContactPersonName";
            ViewData["ContactPersonEmailSortParm"] = sort == "ContactPersonEmail" ? "ContactPersonEmail_desc" : "ContactPersonEmail";
            ViewData["VatSortParm"] = sort == "Vat" ? "Vat_desc" : "Vat";


            if (string.IsNullOrEmpty(sort))
            {
                sort = "Name";
            }
            var customersQuery = _context.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                customersQuery = customersQuery.Where(c =>
                    c.Name.Contains(searchString) ||
                    (c.Vat.HasValue && c.Vat.Value.ToString().Contains(searchString))
                );
            }

            var sortMapping = new Dictionary<string, Expression<Func<Customer, object>>>
            {
                ["Name"] = c => c.Name,
               ["Address"] = c => c.Address,
               ["Telephone"] = c => c.TelephoneNumber,
               ["ContactPersonName"] = c => c.ContactPersonName,
               ["ContactPersonEmail"] = c => c.ContactPersonEmail,
               ["Vat"] = c => c.Vat
            };
            var sortOrder = sort.Replace("_desc","");
            if(sortMapping.ContainsKey(sortOrder))
            {
                if (sort.EndsWith("_desc"))
                {
                    customersQuery = customersQuery.OrderByDescending(sortMapping[sortOrder]);
                }
                else
                {
                    customersQuery = customersQuery.OrderBy(sortMapping[sortOrder]);
                }
            }
            else
            {
                customersQuery = customersQuery.OrderBy(c => c.Name);
            }

            // try
            // {
            //     customersQuery = customersQuery.OrderBy(sort);
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError(ex, "Error sorting customers by {Sort}", sort);
            //     customersQuery = customersQuery.OrderBy(c => c.Name);
            // }

            int pageSize = 10;
            int currentPageNumber = pageNumber ?? 1;
            ViewData["CurrentPage"] = currentPageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)await customersQuery.CountAsync() / pageSize);
            ViewData["TotalCustomers"] = await customersQuery.CountAsync();

            var paginatedCustomers = await customersQuery
                .Skip((currentPageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // var customers = await _context.Customers.ToListAsync();
            return View(paginatedCustomers);
        }


        [HttpGet]
        public IActionResult CustomerForm(int? id)
        {
            if(id.HasValue) {
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id.Value);
                if (customer != null)
                {
                    var customerRequest = new CustomerRequest
                    {
                        CustomerId = customer.CustomerId,
                        Name = customer.Name,
                        Address = customer.Address,
                        TelephoneNumber = customer.TelephoneNumber,
                        ContactPersonName = customer.ContactPersonName,
                        ContactPersonEmail = customer.ContactPersonEmail,
                        Vat = customer.Vat
                    };
                    return View(customerRequest);
                }
                return NotFound();
            }

            return View(new CustomerRequest());
        }

        [HttpPost]
        public async Task<IActionResult> CustomerForm(CustomerRequest res)
        {
            if (ModelState.IsValid)
            {
                if (res.CustomerId.HasValue && res.CustomerId.Value != 0)
                {
                    var existingCustomer = await _context.Customers.FindAsync(res.CustomerId.Value);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }

                    existingCustomer.Name = res.Name;
                    existingCustomer.Address = res.Address;
                    existingCustomer.TelephoneNumber = res.TelephoneNumber;
                    existingCustomer.ContactPersonName = res.ContactPersonName;
                    existingCustomer.ContactPersonEmail = res.ContactPersonEmail;
                    existingCustomer.Vat = res.Vat;

                }
                else
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

                }
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
