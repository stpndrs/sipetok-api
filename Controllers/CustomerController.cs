using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.Utils;
using sipetok_api.dto.Respon;

[Route("api/customers")]
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
                success = true,
                data = allCustomer,
                message = "Successfully retrieved all customer data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<CustomerRespon>>
            {
                success = false,
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

            if (customer == null)
            {
                return NotFound(new ResponData<CustomerRespon>
                {
                    success = false,
                    message = $"Customer data with id {id} not found"
                });
            }

            var respon = new ResponData<CustomerRespon>
            {
                success = true,
                data = customer,
                message = $"Successfully retrieved customer data with id {id}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddCustomer([FromBody] CustomerDto customerDto)
    {
        try
        {
            var user = _mapper.Map<User>(customerDto.user);
            var customer = _mapper.Map<Customer>(customerDto);

            if (customerDto.user != null)
            {
                if (string.IsNullOrWhiteSpace(customerDto.user.password))
                {
                    throw new Exception("Password is required");
                }

                user.username = customerDto.user.username;
                user.password = Bcrypt.BcryptPassword(user.password);
                user.role = 3;
                user.status = 1;
                customer.user = user;
            }

            dbContext.Customers.Add(customer);
            dbContext.SaveChanges();

            var respon = new ResponData<CustomerRespon>
            {
                success = true,
                data = _mapper.Map<CustomerRespon>(customer),
                message = $"Successfully added customer data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
    {
        try
        {
            var customer = dbContext.Customers.Find(id);

            if (customer is null)
            {
                return BadRequest(new ResponData<CustomerRespon>
                {
                    success = false,
                    message = $"Customer data with id {id} not found"
                });
            }

            customer.name = customerDto.name;
            customer.address = customerDto.address;
            customer.phone_number = customerDto.phone_number;
            customer.UpdateTimestamps();

            if (customerDto.user != null)
            {
                var user = dbContext.Users.Find(customer.user_id);
                if (user != null)
                {
                    if(string.IsNullOrWhiteSpace(customerDto.user.password))
                    {
                        throw new Exception("Password is required");
                    }

                    user.username = customerDto.user.username;
                    user.password = Bcrypt.BcryptPassword(customerDto.user.password);
                    user.email = customerDto.user.email;
                    user.status = customerDto.user.status;
                    user.UpdateTimestamps();
                }
            }

            dbContext.SaveChanges();

            var respon = new ResponData<CustomerRespon>
            {
                success = true,
                data = _mapper.Map<CustomerRespon>(customer),
                message = "Successfully updated customer data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public IActionResult DeleteCustomer(int id)
    {
        try
        {
            var customer = dbContext.Customers.Find(id);

            if (customer is null)
            {
                return BadRequest(new ResponData<CustomerRespon>
                {
                    success = false,
                    message = $"Customer data with id {id} not found"
                });
            }

            if(customer.user_id != 0)
            {
                var user = dbContext.Users.Find(customer.user_id);
                if (user != null)
                {
                    user.SoftDelete();
                }
            }

            customer.SoftDelete();
            dbContext.SaveChanges();

            var respon = new ResponData<CustomerRespon>
            {
                success = true,
                message = "Successfully deleted customer data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<CustomerRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }
}