using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using sipetok_api.Controllers;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.service;
using sipetok_api.Utils;
using SipetokTest.Helper;

namespace SipetokTest.Controller
{
    public class TransactionControllerTest
    {
        private readonly Mock<PaymentService> _mockPaymentService;

        public TransactionControllerTest()
        {
            // Karena PaymentService membutuhkan DbContext, kita kirimkan DbContext dummy ke constructor mock-nya
            var dbContext = TestHelper.CreateDbContext();
            _mockPaymentService = new Mock<PaymentService>(dbContext);
        }

        [Fact]
        public void GetAll_ReturnsOk_FunctionCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();

            dbContext.Transactions.Add(new Transaction { id = 1, customer_name = "Stevan", total_price = 50000 });
            dbContext.SaveChanges();

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

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

            var request = new TransactionDto { customer_name = "Andreas", total_price = 100000 };
            var fakeTransaction = new Transaction { id = 10, customer_name = "Andreas" };

            // Mocking logic: Service berhasil memproses transaksi
            _mockPaymentService.Setup(s => s.ProcessTransaction(It.IsAny<TransactionDto>()))
                               .ReturnsAsync(fakeTransaction);

            // Simpan data di in-memory agar query FirstOrDefault di controller berhasil (Line Coverage)
            dbContext.Transactions.Add(fakeTransaction);
            await dbContext.SaveChangesAsync();

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

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

            // Mocking logic: Paksa service melempar error untuk mengetes blok catch (Line/Branch Coverage)
            _mockPaymentService.Setup(s => s.ProcessTransaction(It.IsAny<TransactionDto>()))
                               .ThrowsAsync(new System.Exception("Database Error"));

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

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
            int trxId = 1;

            var trx = new Transaction { id = trxId, Status = PaymentState.Pending };
            dbContext.Transactions.Add(trx);
            await dbContext.SaveChangesAsync();

            _mockPaymentService.Setup(s => s.UpdateStatus(trxId, "NEXT"))
                               .ReturnsAsync(true);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

            // Act
            var result = await controller.Pay(trxId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Pay_ReturnsBadRequest_WhenServiceFails_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();

            // Mocking logic: Simulasikan UpdateStatus gagal (return false)
            _mockPaymentService.Setup(s => s.UpdateStatus(It.IsAny<int>(), "NEXT"))
                               .ReturnsAsync(false);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

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
            int trxId = 2;

            var trx = new Transaction { id = trxId, Status = PaymentState.Pending };
            dbContext.Transactions.Add(trx);
            await dbContext.SaveChangesAsync();

            _mockPaymentService.Setup(s => s.UpdateStatus(trxId, "CANCEL"))
                               .ReturnsAsync(true);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

            // Act
            var result = await controller.Cancel(trxId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Cancel_ReturnsBadRequest_WhenFails_BranchCoverage()
        {
            // Arrange
            var dbContext = TestHelper.CreateDbContext();
            var mapper = TestHelper.CreateMapper();

            _mockPaymentService.Setup(s => s.UpdateStatus(It.IsAny<int>(), "CANCEL"))
                               .ReturnsAsync(false);

            var controller = new TransactionController(dbContext, _mockPaymentService.Object, mapper);

            // Act
            var result = await controller.Cancel(999);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}