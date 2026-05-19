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
            var allCustomer = _mapper.Map<List<CustomerRespon>>(await dbContext.Customers.Include(c => c.user).ToListAsync());

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
            var customer = _mapper.Map<CustomerRespon>(await dbContext.Customers.Include(c => c.user).FirstOrDefaultAsync(c => c.id == id));

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
            var customer = _mapper.Map<CustomerRespon>(await dbContext.Customers.FirstOrDefaultAsync(c => c.user_id == userId));

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

            if (customerDto.user != null)
            {
                var user = _mapper.Map<User>(customerDto.user);
                if (string.IsNullOrWhiteSpace(customerDto.user.password))
                {
                    throw new Exception("Password is required");
                }

                user.password = Bcrypt.BcryptPassword(user.password);
                user.role = 3;
                user.status = 1;
                customer.user = user;
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

            if (customerDto.user != null)
            {
                var user = await dbContext.Users.FindAsync(customer.user_id);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(customerDto.user.password))
                    {
                        user.password = Bcrypt.BcryptPassword(customerDto.user.password);
                    }

                    _mapper.Map(customerDto.user, user);
                    user.UpdateTimestamps();
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
            var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.user_id == userId);

            if (customer is null)
            {
                return BadRequest(new ResponData<object?>(false, $"Customer data with user id {userId} not found"));
            }

            _mapper.Map(customerDto, customer);
            customer.UpdateTimestamps();

            if (customerDto.user != null)
            {
                var user = await dbContext.Users.FindAsync(customer.user_id);
                if (user != null)
                {
                    _mapper.Map(customerDto.user, user);
                    if (!string.IsNullOrWhiteSpace(customerDto.user.password))
                    {
                        user.password = Bcrypt.BcryptPassword(customerDto.user.password);
                    }

                    user.role = 3;
                    user.status = 1;
                    user.UpdateTimestamps();
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

            if (customer.user_id != 0)
            {
                var user = await dbContext.Users.FindAsync(customer.user_id);
                if (user != null)
                {
                    user.SoftDelete();
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