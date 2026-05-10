using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using Microsoft.EntityFrameworkCore;
using sipetok_api.dto.Request;
using sipetok_api.Models;


[Authorize]
[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper _mapper;

    public TransactionController(AppDbContext context, IMapper mapper)
    {
        dbContext = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "1")]
    public IActionResult GetAllTransactions()
    {
        try
        {
            var allTransaction = _mapper.Map<List<TransactionRespon>>(dbContext.Transactions.Include(c => c.tenant).ToList());

            var respon = new ResponData<List<TransactionRespon>>
            {
                success = true,
                data = allTransaction,
                message = "Successfully retrieved all Transaction data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<TransactionRespon>>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Roles = "1, 2")]
    public IActionResult GetTransactionById(int id)
    {
        try
        {
            var transaction = _mapper.Map<TransactionRespon>(dbContext.Transactions.Include(c => c.tenant).FirstOrDefault(c => c.id == id));

            if (transaction == null)
            {
                return NotFound(new ResponData<TransactionRespon>
                {
                    success = false,
                    message = $"Transaction data with id {id} not found"
                });
            }

            var respon = new ResponData<TransactionRespon>
            {
                success = true,
                data = transaction,
                message = $"Successfully retrieved transaction data with id {id}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TransactionRespon>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("tenant/{tenantId:int}")]
    [Authorize(Roles = "1, 2")]
    public IActionResult GetTransactionByTenantId(int tenantId)
    {
        try
        {
            var transaction = _mapper.Map<List<TransactionRespon>>(
                dbContext.Transactions.Include(o => o.tenant).Where(o => o.tenant_id == tenantId).ToList()
            );

            if (transaction == null)
            {
                return NotFound(new ResponData<TransactionRespon>
                {
                    success = false,
                    message = $"Transaction data with tenant id {tenantId} not found"
                });
            }

            var respon = new ResponData<List<TransactionRespon>>
            {
                success = true,
                data = transaction,
                message = $"Successfully retrieved transaction data with tenant id {tenantId}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TransactionRespon>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("mytransactions")]
    [Authorize(Roles = "2")]
    public IActionResult GetMyTransactions()
    {
        try
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0");
            var transaction = _mapper.Map<List<TransactionRespon>>
            (
                (from o in dbContext.Transactions
                 join t in dbContext.Tenants on o.tenant_id equals t.id
                 where t.user_id == userId
                 select o
                 ).ToList()
            );

            if (transaction == null)
            {
                return NotFound(new ResponData<TransactionRespon>
                {
                    success = false,
                    message = $"Transaction data with user id {userId} not found"
                });
            }

            var respon = new ResponData<List<TransactionRespon>>
            {
                success = true,
                data = transaction,
                message = $"Successfully retrieved transaction data with user id {userId}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TransactionRespon>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    [Route("addmytransactions")]
    [Authorize(Roles = "2")]
    public IActionResult AddMyTransactions([FromBody] TransactionDto transactionDto)
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId));

            if (tenant is null)
            {
                return BadRequest(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = "Tenant not found"
                    }
                );
            }
            if (transactionDto.details.Count == 0)
            {
                return BadRequest(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = "Invalid transaction data"
                    }
                );
            }

            foreach (var detail in transactionDto.details)
            {
                ReduceEggStock(detail.category_name, detail.quantity, tenant.id);
            }

            var transaction = _mapper.Map<Transaction>(transactionDto);
            transaction.tenant_id = tenant.id;

            dbContext.Transactions.Add(transaction);
            dbContext.SaveChanges();

            var respon = new ResponData<TransactionRespon>
            {
                success = true,
                data = _mapper.Map<TransactionRespon>(transaction),
                message = "Successfully added transaction data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TransactionRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "2")]
    public IActionResult UpdateTransaction(int id, [FromBody] TransactionDto transactionDto)
    {
        try
        {
            var transaction = dbContext.Transactions.Find(id);
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId));

            if (tenant is null)
            {
                return BadRequest(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = "Tenant not found"
                    }
                );
            }
            if (transaction is null)
            {
                return NotFound(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = $"Transaction data with id {id} not found"
                    }
                );
            }
            if (transaction.tenant_id != tenant.id)
            {
                return BadRequest(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = "You are not authorized to update this transaction data"
                    }
                );
            }

            _mapper.Map(transactionDto, transaction);
            transaction.UpdateTimestamps();

            dbContext.SaveChanges();

            var respon = new ResponData<TransactionRespon>
            {
                success = true,
                data = _mapper.Map<TransactionRespon>(transaction),
                message = "Successfully updated transaction data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TransactionRespon>
            {
                success = false,
                message = ex.Message
            };

            return Ok(respon);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public IActionResult DeleteTransaction(int id)
    {
        try
        {
            var transaction = dbContext.Transactions.Find(id);
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId));

            if (tenant is null)
            {
                return BadRequest(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = "Tenant not found"
                    }
                );
            }
            if (transaction is null)
            {
                return NotFound(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = $"Transaction data with id {id} not found"
                    }
                );
            }
            if (transaction.tenant_id != tenant.id)
            {
                return BadRequest(
                    new ResponData<TransactionRespon>
                    {
                        success = false,
                        message = "You are not authorized to update this transaction data"
                    }
                );
            }

            transaction.SoftDelete();
            dbContext.SaveChanges();

            var respon = new ResponData<TransactionRespon>
            {
                success = true,
                message = "Successfully deleted transaction data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TransactionRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    private void ReduceEggStock(string categoryName, double quantity, int tenantId)
    {
        var category = dbContext.EggCategories
            .FirstOrDefault(c => c.name == categoryName && c.tenant_id == tenantId);

        if (category == null)
        {
            throw new Exception($"Kategori '{categoryName}' tidak ditemukan.");
        }

        var eggStocks = dbContext.Eggs
            .Where(e => e.category_id == category.id && e.tenant_id == tenantId && e.stock > 0)
            .OrderBy(e => e.production_date)
            .ToList();

        double remainingToReduce = quantity;

        if (eggStocks.Sum(e => e.stock) < quantity)
        {
            throw new Exception($"Total stok untuk '{categoryName}' tidak mencukupi permintaan.");
        }

        foreach (var egg in eggStocks)
        {
            if (remainingToReduce <= 0) break;

            if (egg.stock >= remainingToReduce)
            {
                egg.stock -= (int)remainingToReduce;
                remainingToReduce = 0;
            }
            else
            {
                remainingToReduce -= egg.stock;
                egg.stock = 0;
            }

            egg.UpdateTimestamps();
        }
    }
}