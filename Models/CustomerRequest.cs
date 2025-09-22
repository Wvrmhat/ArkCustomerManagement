using System.ComponentModel.DataAnnotations;


namespace ArkCustomerManagement.Models;

public class CustomerRequest
{
    public int? CustomerId { get; set; }

    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }

    public int? TelephoneNumber { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactPersonEmail { get; set; }

    public int? Vat { get; set; }
}
