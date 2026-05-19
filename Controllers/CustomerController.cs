using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using sipetok_api.dto.Respon;

[Authorize]
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
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllCustomers()
    {
        try
        {
            var allCustomer = _mapper.Map<List<CustomerRespon>>(await dbContext.Customers.Include(c => c.User).ToListAsync());

            var respon = new ResponData<List<CustomerRespon>>(true, allCustomer, "Successfully retrieved all customer data");

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        try
        {
            var customer = _mapper.Map<CustomerRespon>(await dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id));

            if (customer == null)
            {
                return NotFound(new ResponData<object?>(false, $"Customer data with id {id} not found"));
            }

            var respon = new ResponData<CustomerRespon>(true, customer, $"Successfully retrieved customer data with id {id}");

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("myprofile")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var customer = _mapper.Map<CustomerRespon>(await dbContext.Customers.FirstOrDefaultAsync(c => c.UserId == userId));

            if (customer == null)
            {
                return NotFound(new ResponData<object?>(false, $"Customer data with id {userId} not found"));
            }

            var respon = new ResponData<CustomerRespon>(true, customer, $"Successfully retrieved customer data with id {userId}");

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
    {
        try
        {
            var customer = _mapper.Map<Customer>(customerDto);

            if (customerDto.User != null)
            {
                var User = _mapper.Map<User>(customerDto.User);
                if (string.IsNullOrWhiteSpace(customerDto.User.Password))
                {
                    throw new Exception("Password is required");
                }

                User.Password = Bcrypt.BcryptPassword(User.Password);
                User.Role = 3;
                User.Status = 1;
                customer.User = User;
            }

            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();

            var respon = new ResponData<CustomerRespon>(true, _mapper.Map<CustomerRespon>(customer), "Successfully added new customer data");

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
    {
        try
        {
            var customer = await dbContext.Customers.FindAsync(id);

            if (customer is null)
            {
                return BadRequest(new ResponData<object?>(false, $"Customer data with id {id} not found"));
            }

            _mapper.Map(customerDto, customer);
            customer.UpdateTimestamps();

            if (customerDto.User != null)
            {
                var User = await dbContext.Users.FindAsync(customer.UserId);
                if (User != null)
                {
                    if (!string.IsNullOrWhiteSpace(customerDto.User.Password))
                    {
                        User.Password = Bcrypt.BcryptPassword(customerDto.User.Password);
                    }

                    _mapper.Map(customerDto.User, User);
                    User.UpdateTimestamps();
                }
            }
            await dbContext.SaveChangesAsync();

            var respon = new ResponData<CustomerRespon>(true, _mapper.Map<CustomerRespon>(customer), "Successfully updated customer data");
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("updatemyprofile")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] CustomerDto customerDto)
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer is null)
            {
                return BadRequest(new ResponData<object?>(false, $"Customer data with User id {userId} not found"));
            }

            _mapper.Map(customerDto, customer);
            customer.UpdateTimestamps();

            if (customerDto.User != null)
            {
                var User = await dbContext.Users.FindAsync(customer.UserId);
                if (User != null)
                {
                    _mapper.Map(customerDto.User, User);
                    if (!string.IsNullOrWhiteSpace(customerDto.User.Password))
                    {
                        User.Password = Bcrypt.BcryptPassword(customerDto.User.Password);
                    }

                    User.Role = 3;
                    User.Status = 1;
                    User.UpdateTimestamps();
                }
            }

            await dbContext.SaveChangesAsync();

            var respon = new ResponData<CustomerRespon>(true, _mapper.Map<CustomerRespon>(customer), "Successfully updated my profile");

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return BadRequest(respon);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var customer = await dbContext.Customers.FindAsync(id);

            if (customer is null)
            {
                return BadRequest(new ResponData<object?>(false, $"Customer data with id {id} not found"));
            }

            if (customer.UserId != 0)
            {
                var User = await dbContext.Users.FindAsync(customer.UserId);
                if (User != null)
                {
                    User.SoftDelete();
                }
            }

            customer.SoftDelete();
            await dbContext.SaveChangesAsync();

            var respon = new ResponData<string?>(true, null, "Successfully deleted customer data");

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<object?>(false, ex.Message);

            return BadRequest(respon);
        }
    }
}