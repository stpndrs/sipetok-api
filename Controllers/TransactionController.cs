using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionFactory _factory;

        public TransactionController(TransactionFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (GetData)_factory.CreateMethod("get");

            return await handler.ActionAsync<Transaction, TransactionRespon>("tx_all_tenant", userId: userId);
        }

        [HttpPost]
        public async Task<IActionResult> Store([FromBody] TransactionDto transactionDto)
        {
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Transaction, TransactionDto, TransactionRespon>(
                subAction: "tx_store",
                data: transactionDto,
                httpMethod: "POST"
            );
        }

        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id, [FromBody] PaymentDto paymentDto)
        {
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Transaction, PaymentDto, TransactionRespon>(
                subAction: "tx_pay",
                data: paymentDto,
                httpMethod: "POST",
                id: id
            );
        }

        [HttpPost("cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Transaction, object, TransactionRespon>(
                subAction: "tx_cancel",
                data: new object(),
                httpMethod: "POST",
                id: id
            );
        }

        [HttpPost("complete/{id:int}")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Transaction, object, TransactionRespon>(
                subAction: "tx_complete",
                data: new object(),
                httpMethod: "POST",
                id: id
            );
        }
    }
}