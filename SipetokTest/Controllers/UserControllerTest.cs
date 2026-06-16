// //UserControllerTest

// using Microsoft.AspNetCore.Mvc;
// using sipetok_api.dto.Request;
// using sipetok_api.Models;
// using SipetokTest.Helper;
// namespace SipetokTest.Controller
// {
//     public class UserControllerTest
//     {
//         [Fact]
//         public void GetAllUsers_ReturnsOk()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             dbContext.Users.Add(new User
//             {
//                 username = "admin",
//                 email = "admin@gmail.com",
//                 password = Bcrypt.HashPassword("password123"),
//                 role = 1,
//                 status = 1
//             });
//             dbContext.SaveChanges();

//             var controller = new global::UserController(dbContext, mapper);

//             // Act
//             var result = controller.GetAllUsers();

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void GetUserById_ReturnsOk_WhenUserExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var user = new User
//             {
//                 username = "user1",
//                 email = "user1@gmail.com",
//                 password = Bcrypt.HashPassword("password123"),
//                 role = 3,
//                 status = 1
//             };

//             dbContext.Users.Add(user);
//             dbContext.SaveChanges();

//             var controller = new global::UserController(dbContext, mapper);

//             // Act
//             var result = controller.GetUserById(user.id);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::UserController(dbContext, mapper);

//             // Act
//             var result = controller.GetUserById(999);

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }

//         [Fact]
//         public void GetMyAccount_ReturnsOk_WhenUserExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var user = new User
//             {
//                 username = "myaccount",
//                 email = "myaccount@gmail.com",
//                 password = Bcrypt.HashPassword("password123"),
//                 role = 3,
//                 status = 1
//             };

//             dbContext.Users.Add(user);
//             dbContext.SaveChanges();

//             var controller = new global::UserController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.id);

//             // Act
//             var result = controller.GetMyAccount();

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void GetMyAccount_ReturnsNotFound_WhenUserDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var controller = new global::UserController(dbContext, mapper);
//             TestHelper.SetUserId(controller, 999);

//             // Act
//             var result = controller.GetMyAccount();

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }

//         [Fact]
//         public void AddUser_ReturnsOk_WhenDataValid()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::UserController(dbContext, mapper);

//             var request = new UserRequestDto
//             {
//                 username = "userbaru",
//                 email = "userbaru@gmail.com",
//                 password = "password123",
//                 role = 3,
//                 status = 1
//             };

//             // Act
//             var result = controller.AddUser(request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.Single(dbContext.Users);
//         }

//         [Fact]
//         public void AddUser_ReturnsBadRequest_WhenPasswordEmpty()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::UserController(dbContext, mapper);

//             var request = new UserRequestDto
//             {
//                 username = "userbaru",
//                 email = "userbaru@gmail.com",
//                 password = "",
//                 role = 3,
//                 status = 1
//             };

//             // Act
//             var result = controller.AddUser(request);

//             // Assert
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//         [Fact]
//         public void UpdateUser_ReturnsOk_WhenUserExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var user = new User
//             {
//                 username = "userlama",
//                 email = "lama@gmail.com",
//                 password = Bcrypt.HashPassword("password123"),
//                 role = 3,
//                 status = 1
//             };

//             dbContext.Users.Add(user);
//             dbContext.SaveChanges();

//             var controller = new global::UserController(dbContext, mapper);

//             var request = new UserRequestDto
//             {
//                 username = "userbaru",
//                 email = "baru@gmail.com",
//                 password = "passwordbaru",
//                 role = 3,
//                 status = 1
//             };

//             // Act
//             var result = controller.UpdateUser(user.id, request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::UserController(dbContext, mapper);

//             var request = new UserRequestDto
//             {
//                 username = "userbaru",
//                 email = "baru@gmail.com",
//                 password = "passwordbaru",
//                 role = 3,
//                 status = 1
//             };

//             // Act
//             var result = controller.UpdateUser(999, request);

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }

//         [Fact]
//         public void UpdateMyAccount_ReturnsOk_WhenUserExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var user = new User
//             {
//                 username = "myaccount",
//                 email = "old@gmail.com",
//                 password = Bcrypt.HashPassword("password123"),
//                 role = 3,
//                 status = 1
//             };

//             dbContext.Users.Add(user);
//             dbContext.SaveChanges();

//             var controller = new global::UserController(dbContext, mapper);
//             TestHelper.SetUserId(controller, user.id);

//             var request = new UserRequestDto
//             {
//                 username = "myaccountnew",
//                 email = "new@gmail.com",
//                 password = "passwordbaru",
//                 role = 3,
//                 status = 1
//             };

//             // Act
//             var result = controller.UpdateMyAccount(request);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//         }

//         [Fact]
//         public void UpdateMyAccount_ReturnsNotFound_WhenUserDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var controller = new global::UserController(dbContext, mapper);
//             TestHelper.SetUserId(controller, 999);

//             var request = new UserRequestDto
//             {
//                 username = "myaccountnew",
//                 email = "new@gmail.com",
//                 password = "passwordbaru",
//                 role = 3,
//                 status = 1
//             };

//             // Act
//             var result = controller.UpdateMyAccount(request);

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }

//         [Fact]
//         public void DeleteUser_ReturnsOk_WhenUserExists()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();

//             var user = new User
//             {
//                 username = "deleteuser",
//                 email = "delete@gmail.com",
//                 password = Bcrypt.HashPassword("password123"),
//                 role = 3,
//                 status = 1
//             };

//             dbContext.Users.Add(user);
//             dbContext.SaveChanges();

//             var controller = new global::UserController(dbContext, mapper);

//             // Act
//             var result = controller.DeleteUser(user.id);

//             // Assert
//             Assert.IsType<OkObjectResult>(result);
//             Assert.NotNull(user.deleted_at);
//         }

//         [Fact]
//         public void DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
//         {
//             // Arrange
//             var dbContext = TestHelper.CreateDbContext();
//             var mapper = TestHelper.CreateMapper();
//             var controller = new global::UserController(dbContext, mapper);

//             // Act
//             var result = controller.DeleteUser(999);

//             // Assert
//             Assert.IsType<NotFoundObjectResult>(result);
//         }
//     }
// }
