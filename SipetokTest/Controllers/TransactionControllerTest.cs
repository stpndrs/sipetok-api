using Microsoft.AspNetCore.Mvc;
using Moq;
using sipetok_api.Controllers;
using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Services;
using sipetok_api.Utils;
using SipetokTest.Helper;

namespace SipetokTest.Controller
{
    public class TransactionControllerTest
    {
        private readonly Mock<PaymentService> _mockPaymentService;

        public TransactionControllerTest()
        {
            var dbContext = TestHelper.CreateDbContext();
            _mockPaymentService = new Mock<PaymentService>(dbContext);
        }

        [Fact]
        public void GetAll_ReturnsOk_FunctionCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();

            dbContext.Transactions.Add(new Transaction
            {
                id = 1,
                customer_name = "Stevan",
                total_price = 50000
            });
            dbContext.SaveChanges();

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Store_ReturnsOk_WhenDataValid_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();

            var request = new TransactionDto
            {
                customer_name = "Andreas",
                total_price = 100000
            };

            var fakeTransaction = new Transaction
            {
                id = 10,
                customer_name = "Andreas",
                OrderStatus = OrderState.OrderComeIn
            };

            _mockPaymentService.Setup(s => s.ProcessTransaction(It.IsAny<TransactionDto>()))
                               .ReturnsAsync(fakeTransaction);

            dbContext.Transactions.Add(fakeTransaction);
            await dbContext.SaveChangesAsync();

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = await controller.Store(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Store_ReturnsBadRequest_WhenExceptionThrown_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();

            _mockPaymentService.Setup(s => s.ProcessTransaction(It.IsAny<TransactionDto>()))
                               .ThrowsAsync(new Exception("Database Error"));

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = await controller.Store(new TransactionDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Pay_ReturnsOk_WhenSuccess_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();
            int trxId = 1;

            var trx = new Transaction
            {
                id = trxId,
                Status = PaymentState.WaitingForPayment,
                OrderStatus = OrderState.OrderComeIn
            };

            dbContext.Transactions.Add(trx);
            await dbContext.SaveChangesAsync();

            // Setup Mock dengan 4 parameter & Callback untuk mensimulasikan perubahan state asli di dalam Service
            _mockPaymentService.Setup(s => s.UpdateStatus(trxId, PaymentTrigger.Pay, It.IsAny<OrderService>(), OrderTrigger.PaymentSucceeded))
                               .Callback<int, PaymentTrigger, OrderService, OrderTrigger>((id, pt, os, ot) => {
                                   trx.Status = PaymentState.Success;
                                   trx.OrderStatus = OrderState.ReadyForPickup;
                               })
                               .ReturnsAsync(true);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = await controller.Pay(trxId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(OrderState.ReadyForPickup, trx.OrderStatus);
            Assert.Equal(PaymentState.Success, trx.Status);
        }

        [Fact]
        public async Task Pay_ReturnsBadRequest_WhenServiceFails_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();

            // Setup Mock dengan 4 parameter
            _mockPaymentService.Setup(s => s.UpdateStatus(It.IsAny<int>(), PaymentTrigger.Pay, It.IsAny<OrderService>(), OrderTrigger.PaymentSucceeded))
                               .ReturnsAsync(false);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = await controller.Pay(999);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Cancel_ReturnsOk_WhenSuccess_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();
            int trxId = 2;

            var trx = new Transaction
            {
                id = trxId,
                Status = PaymentState.WaitingForPayment,
                OrderStatus = OrderState.OrderComeIn
            };

            dbContext.Transactions.Add(trx);
            await dbContext.SaveChangesAsync();

            // Setup Mock dengan 4 parameter menggunakan Trigger Cancel
            _mockPaymentService.Setup(s => s.UpdateStatus(trxId, PaymentTrigger.Cancel, It.IsAny<OrderService>(), OrderTrigger.CancelledByCustomer))
                               .Callback<int, PaymentTrigger, OrderService, OrderTrigger>((id, pt, os, ot) => {
                                   trx.Status = PaymentState.Cancelled;
                                   trx.OrderStatus = OrderState.Cancelled;
                               })
                               .ReturnsAsync(true);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = await controller.Cancel(trxId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(OrderState.Cancelled, trx.OrderStatus);
            Assert.Equal(PaymentState.Cancelled, trx.Status);
        }

        [Fact]
        public async Task Cancel_ReturnsBadRequest_WhenFails_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();
            var orderService = new OrderService();

            // Setup Mock dengan 4 parameter
            _mockPaymentService.Setup(s => s.UpdateStatus(It.IsAny<int>(), PaymentTrigger.Cancel, It.IsAny<OrderService>(), OrderTrigger.CancelledByCustomer))
                               .ReturnsAsync(false);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, orderService, mapper);

            // Act
            var result = await controller.Cancel(999);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}