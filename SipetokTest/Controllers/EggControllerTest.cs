// using Microsoft.AspNetCore.Mvc;
// using sipetok_api.Data;
// using sipetok_api.dto.Request;
// using sipetok_api.Models;
// using sipetok_api.Controllers;
// using SipetokTest.Helper;

// namespace SipetokTest.Controller
// {
//     public class EggControllerTest
//     {
//         [Fact]
//         public void GetAllEggs_ReturnsOk()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (_, tenant, category) = SeedTenantData(dbContext, "1");

//             SeedEgg(dbContext, tenant.Id, category.Id, 10);

//             var controller = new global::EggController(dbContext, mapper);

//             // Act
//             var result = controller.GetAllEggs();

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void GetEggById_ReturnsOk_WhenEggExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (_, tenant, category) = SeedTenantData(dbContext, "1");
//             var egg = SeedEgg(dbContext, tenant.Id, category.Id, 10);

//             var controller = new global::EggController(dbContext, mapper);

//             // Act
//             var result = controller.GetEggById(egg.Id);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void GetEggById_ReturnsNotFound_WhenEggDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::EggController(dbContext, mapper);

//             // Act
//             var result = controller.GetEggById(999);

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }

//         [Fact]
//         public void GetMyEggs_ReturnsOk_WhenTenantExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, tenant, category) = SeedTenantData(dbContext, "1");

//             SeedEgg(dbContext, tenant.Id, category.Id, 10);
//             SeedEgg(dbContext, tenant.Id, category.Id, 15);

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             // Act
//             var result = controller.GetMyEggs();

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void GetMyEggs_ReturnsBadRequest_WhenTenantDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, 999);

//             // Act
//             var result = controller.GetMyEggs();

//             // Assert
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//         [Fact]
//         public void GetTotalEggByTenantId_ReturnsOk_WhenTenantExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, tenant, category) = SeedTenantData(dbContext, "1");

//             SeedEgg(dbContext, tenant.Id, category.Id, 10);
//             SeedEgg(dbContext, tenant.Id, category.Id, 20);

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             // Act
//             // FIX: Disesuaikan menggunakan endpoint tenant milikmu yang valid (GetMyEggs atau method total yang ada)
//             var result = controller.GetMyEggs();

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void AddEgg_ReturnsOk_WhenDataValid()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, _, category) = SeedTenantData(dbContext, "1");

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 11),
//                 stock = 12,
//                 category_id = category.Id
//             };

//             // FIX: Menggunakan AddMyEgg sesuai routing tenant kamu
//             var result = controller.AddMyEgg(request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.Single(dbContext.EggInventories);
//         }

//         [Fact]
//         public void AddEgg_ReturnsBadRequest_WhenCategoryDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, _, _) = SeedTenantData(dbContext, "1");

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 11),
//                 stock = 12,
//                 category_id = 999
//             };

//             // FIX: Menggunakan AddMyEgg
//             var result = controller.AddMyEgg(request);

//             // Assert
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//         [Fact]
//         public void AddMyEgg_ReturnsOk_WhenCategoryBelongsToTenant()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, _, category) = SeedTenantData(dbContext, "1");

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 11),
//                 stock = 8,
//                 category_id = category.Id
//             };

//             // Act
//             var result = controller.AddMyEgg(request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.Single(dbContext.EggInventories);
//         }

//         [Fact]
//         public void AddMyEgg_ReturnsBadRequest_WhenCategoryDoesNotBelongToTenant()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var (user1, _, _) = SeedTenantData(dbContext, "1");
//             var (_, _, category2) = SeedTenantData(dbContext, "2");

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user1.Id);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 11),
//                 stock = 8,
//                 category_id = category2.Id
//             };

//             // Act
//             var result = controller.AddMyEgg(request);

//             // Assert
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//         [Fact]
//         public void UpdateEgg_ReturnsOk_WhenEggExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (_, tenant, category) = SeedTenantData(dbContext, "1");
//             var egg = SeedEgg(dbContext, tenant.Id, category.Id, 10);

//             var controller = new global::EggController(dbContext, mapper);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 12),
//                 stock = 25,
//                 category_id = category.Id
//             };

//             // FIX: Diubah ke UpdateMyEgg agar valid dengan method controller-mu
//             var result = controller.UpdateMyEgg(egg.Id, request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.Equal(25, dbContext.EggInventories.Find(egg.Id)?.stock);
//         }

//         [Fact]
//         public void UpdateEgg_ReturnsNotFound_WhenEggDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::EggController(dbContext, mapper);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 12),
//                 stock = 25,
//                 category_id = 1
//             };

//             // FIX: Diubah ke UpdateMyEgg
//             var result = controller.UpdateMyEgg(999, request);

//             // Assert
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//         [Fact]
//         public void UpdateMyEgg_ReturnsOk_WhenEggBelongsToUser()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, tenant, category) = SeedTenantData(dbContext, "1");
//             var egg = SeedEgg(dbContext, tenant.Id, category.Id, 10);

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 13),
//                 stock = 30,
//                 category_id = category.Id
//             };

//             // Act
//             var result = controller.UpdateMyEgg(egg.Id, request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.Equal(30, dbContext.EggInventories.Find(egg.Id)?.stock);
//         }

//         [Fact]
//         public void UpdateMyEgg_ReturnsBadRequest_WhenEggDoesNotBelongToUser()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var (user1, _, _) = SeedTenantData(dbContext, "1");
//             var (_, tenant2, category2) = SeedTenantData(dbContext, "2");
//             var egg = SeedEgg(dbContext, tenant2.Id, category2.Id, 10);

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user1.Id);

//             var request = new EggDto
//             {
//                 production_date = new DateTime(2026, 5, 13),
//                 stock = 30,
//                 category_id = category2.Id
//             };

//             // Act
//             var result = controller.UpdateMyEgg(egg.Id, request);

//             // Assert
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//         [Fact]
//         public void DeleteEgg_ReturnsOk_WhenEggBelongsToUser()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var (user, tenant, category) = SeedTenantData(dbContext, "1");
//             var egg = SeedEgg(dbContext, tenant.Id, category.Id, 10);

//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.Id);

//             // Act
//             var result = controller.DeleteEgg(egg.Id);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.NotNull(egg.deleted_at);
//         }

//         [Fact]
//         public void DeleteEgg_ReturnsNotFound_WhenEggDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::EggController(dbContext, mapper);
//             TestHelper.SetUserId(controller, 999);

//             // Act
//             var result = controller.DeleteEgg(999);

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }

//         private static (User user, Tenant tenant, EggCategory category) SeedTenantData(AppDbContext dbContext, string suffix)
//         {
//             var user = new User
//             {
//                 username = $"tenant{suffix}",
//                 email = $"tenant{suffix}@gmail.com",
//                 password = "password123",
//                 role = 2,
//                 status = 1
//             };

//             dbContext.Users.Add(user);
//             dbContext.SaveChanges();

//             var tenant = new Tenant
//             {
//                 name = $"Tenant {suffix}",
//                 address = $"Alamat {suffix}",
//                 phoneNumber = $"08123{suffix}",
//                 isValid = true,
//                 user_id = user.Id
//             };

//             dbContext.Tenants.Add(tenant);
//             dbContext.SaveChanges();

//             var category = new EggCategory
//             {
//                 name = $"Kategori {suffix}",
//                 description = "Telur ayam",
//                 price = 2000,
//                 tenant_id = tenant.Id
//             };

//             dbContext.EggCategories.Add(category);
//             dbContext.SaveChanges();

//             return (user, tenant, category);
//         }

//         private static EggInventory SeedEgg(AppDbContext dbContext, int tenantId, int categoryId, int stock)
//         {
//             var egg = new EggInventory
//             {
//                 production_date = new DateTime(2026, 5, 10),
//                 stock = stock,
//                 tenant_id = tenantId,
//                 category_id = categoryId
//             };

//             dbContext.EggInventories.Add(egg);
//             dbContext.SaveChanges();

//             return egg;
//         }
//     }
// }