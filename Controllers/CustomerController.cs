using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.Utils;
using sipetok_api.dto.Respon;

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
        try
        {
            var allCustomer = _mapper.Map<List<CustomerRespon>>(dbContext.Customers.Include(c => c.user).ToList());
            var respon = new ResponData<List<CustomerRespon>>
            {
                status = true,
                data = allCustomer,
                message = "Berhasil mengambil semua data customer"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<CustomerRespon>>
            {
                status = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetCustomerById(int id)
    {
        try
        {
            var customer = _mapper.Map<CustomerRespon>(dbContext.Customers.Include(c => c.user).FirstOrDefault(c => c.id == id));

            if(customer == null)
            {
                return NotFound(new ResponData<CustomerRespon>
                {
                    status = false,
                    message = $"Data customer dengan id {id} tidak ditemukan"
                });
            }

            var respon = new ResponData<CustomerRespon>
            {
                status = true,
                data = customer,
                message = $"Berhasil mengambil data customer pada id {id}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                status = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddCustomer(CustomerDto customerDto)
    {
        try
        {
            if (customerDto.user is null)
            {
                return BadRequest(new ResponData<TenantRespon>
                {
                    status = false,
                    message = "User wajib diisi"
                });
            }

            var customer = _mapper.Map<Customer>(customerDto);
            var user = _mapper.Map<User>(customerDto.user);

            user.password = Bcrypt.BcryptPassword(user.password);
            user.role = 3;
            user.status = 1;
            customer.user = user;

            dbContext.Customers.Add(customer);
            dbContext.SaveChanges();

            var respon = new ResponData<CustomerRespon>
            {
                status = true,
                data = _mapper.Map<CustomerRespon>(customer),
                message = $"Berhasil menambahkan data customer"
            };
            
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                status = false,
                message = ex.Message
            };
            
            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateCustomer(int id, CustomerDto customerDto)
    {
        try
        {
            var customer = dbContext.Customers.Find(id);

            if (customer is null)
            {
                return BadRequest(new ResponData<CustomerRespon>
                {
                    status = false,
                    message = $"Data customer dengan id {id} tidak ditemukan"
                });
            }

            if (!string.IsNullOrEmpty(customerDto.name))
                customer.name = customerDto.name;

            if (!string.IsNullOrEmpty(customerDto.address))
                customer.address = customerDto.address;

            if (!string.IsNullOrEmpty(customerDto.phone_number))
                customer.phone_number = customerDto.phone_number;

            dbContext.SaveChanges();

            var respon = new ResponData<CustomerRespon>
            {
                status = true,
                data = _mapper.Map<CustomerRespon>(customer),
                message = "Berhasil memperbarui data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                status = true,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }
}