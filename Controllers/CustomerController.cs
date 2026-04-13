using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.helper;
using sipetok_api.Models;
using sipetok_api.Data;
using sipetok_api.dto;
using AutoMapper;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper _mapper;

    public CustomerController(AppDbContext context, IMapper mapper)
    {
        dbContext = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllCustomers()
    {
        var allCustomer = dbContext.Customers.Include(c => c.user).ToList();
        return Ok(allCustomer);
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetCustomerById(int id)
    {
        var customer = dbContext.Customers.Include(c => c.user).FirstOrDefault(c => c.id == id);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public IActionResult AddCustomer(CustomerDto customerDto)
    {
        if (customerDto.user == null)
        {
            return BadRequest("User wajib diisi");
        }

        var user = _mapper.Map<User>(customerDto.user);

        user.password = Bcrypt.BcryptPassword(user.password);
        user.role = Role.CUSTOMER;
        user.status = Status.ACTIVE;

        var customer = _mapper.Map<Customer>(customerDto);

        customer.user = user;

        dbContext.Customers.Add(customer);
        dbContext.SaveChanges();

        return Ok(customer);
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateCustomer(int id, CustomerDto customerDto)
    {
        var customer = dbContext.Customers.Find(id);

        if (customer is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(customerDto.name))
            customer.name = customerDto.name;

        if (!string.IsNullOrEmpty(customerDto.address))
            customer.address = customerDto.address;

        if (!string.IsNullOrEmpty(customerDto.phone_number))
            customer.phone_number = customerDto.phone_number;


        dbContext.SaveChanges();
        return Ok(customer);
    }
}