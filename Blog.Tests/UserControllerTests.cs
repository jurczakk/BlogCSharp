using Blog.API.Controllers;
using Blog.API.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void GET_GetById_IncorrectUserId_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.GetById(7777);

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void GET_GetById_CorrectUserId_User()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 1, Name = "Username12", Password = "Username121!", Email = "test@test.com" });
            var User = UserController.GetById(1);

            // Assert
            Assert.IsType<ActionResult<User>>(User);
        }

        [Fact]
        public void POST_Login_UserIsNull_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Login(null);

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void POST_Login_UserNameIsWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Login(new User { Name = " ", Password = "Username121!" });

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void POST_Login_UserPasswordIsWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Login(new User { Name = "Username12", Password = " " });

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void POST_Login_BothUserNameAndPasswordAreWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Login(new User { Name = "", Password = " " });

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void POST_Login_UserWithWrongName_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Login(new User { Name = "INCORRENTUSERNAME154543", Password = "Username121!" });

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void POST_Login_UserWithCorrectNameAndWrongPassword_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 997, Name = "Username12", Password = "Username121@", Email = "email@email.com" });
            var User = UserController.Login(new User { Name = "Username12", Password = "Username121!" });

            // Assert
            Assert.IsType<NotFoundResult>(User.Result);
        }

        [Fact]
        public void POST_Login_UserWithCorrectBothNameAndPassword_NoContent()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 998, Name = "Username12", Password = "Username121!", Email = "email77@email77.com" });
            var User = UserController.Login(new User { Name = "Username12", Password = "Username121!" });

            // Assert
            Assert.IsType<ActionResult<User>>(User);
        }

        [Fact]
        public void POST_Register_NullUser_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(null);

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_NameIsWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 1, Name = " ", Password = "Username121!!!@#", Email = "test!@4art.com" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_PasswordIsWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 1, Name = "Username12", Password = " ", Email = "test!@#4art.com" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_EmailIsWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 1, Name = "Username12", Password = "Username12!@1", Email = "" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_UserNamePasswordEmailAreWhiteSpace_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 1, Name = " ", Password = " ", Email = "" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_CorrectUser_NoContent()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 2, Name = "Username12", Password = "Username12!", Email = "test@test7.com" });

            // Assert
            Assert.IsType<NoContentResult>(User);
        }

        [Fact]
        public void POST_Register_UserWithThisEmailExist_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 6, Name = "Username12", Password = "Username121!", Email = "test@tesT00.com" });
            var User = UserController.Register(new User { Id = 7, Name = "Username123", Password = "Username121!", Email = "test@tesT00.com" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_UserWithThisNameExist_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 8, Name = "UserName5", Password = "UserName51!", Email = "test@test77.com"});
            var User = UserController.Register(new User { Id = 9, Name = "UserName5", Password = "UserName51!", Email = "test@test78.com"});

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        #region NewTests
        [Fact]
        public void POST_Register_LengthOfNameIsLesserThan8_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 1, Name = "qwer", Password = "Qwerty1!", Email = "TEST@TEST.com" });
            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_LengthOfPasswordIsLessThen8_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 2, Name = "Qwerty1234", Password = "Qwert", Email = "TEST@TEST.com" });
            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void POST_Register_LengthOfEmailIsLessThen8_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Register(new User { Id = 3, Name = "Qwerty1234", Password = "Qwerty1!", Email = "T@T.com" });
            // Assert
            Assert.IsType<NotFoundResult>(User);
        }
        #endregion

        [Fact]
        public void PUT_UpdateUser_UpdatedUserIsNull_NotFound()
        {
            // Arrange 
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdateUser(1, null);

            // Assert 
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdatedUserNameIsWhiteSpace_NotFound()
        {
            // Arrange 
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdateUser(1, new User { Id = 1, Name = "", Password = "Username1!!!", Email = "asdf@aer.com"});

            // Assert 
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdatedUserPasswordIsWhiteSpace_NotFound()
        {
            // Arrange 
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdateUser(1, new User { Id = 1, Name = "Username123", Password = " ", Email = "asdf@aer"});

            // Assert 
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdatedUserEmailIsWhiteSpace_NotFound()
        {
            // Arrange 
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdateUser(1, new User { Id = 1, Name = "Username1234", Password = "Username1!!!23", Email = " "});

            // Assert 
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_EveryValueInUpdatedUserIsWhiteSpace_NotFound()
        {
            // Arrange 
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdateUser(1, new User { Id = 1, Name = " ", Password = " ", Email = " " });

            // Assert 
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdateExistingUser_NoContent()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 3, Name = "Qwertyuiopp", Password = "Qwertyuiopp1!", Email = "test@test3333.com" });
            var User = UserController.UpdateUser(3, new User { Id = 3, Name = "Qwertyuiopp2", Password = "Qwertyuiopp2@", Email = "test@test33.com"});

            // Assert
            Assert.IsType<NoContentResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdateNotExistingUser_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdateUser(77, new User { Id = 77, Name = "qwertyuio", Password = "Qwerty!1!!!", Email = "test@test.com"});

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        #region new 

        [Fact]
        public void PUT_UpdateUser_UpdateWithTheSameName_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 4321, Name = "name", Password = "pass", Email = "test@test.com" });
            var User = UserController.UpdateUser(4321, new User { Id = 4321, Name = "name", Password = "qwer", Email = "test21234@test.com" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdateWithTheSameEmail_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 44444, Name = "name1", Password = "pass1", Email = "qwer@qwer.com" });
            var User = UserController.UpdateUser(44444, new User { Id = 44444, Name = "name1", Password = "pass2", Email = "qw3r@QWER.com" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        [Fact]
        public void PUT_UpdateUser_UpdateWithTheSameBothNameAndEmail_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 55555, Name = "name4", Password = "qwe3!", Email = "EMAIL@EMAIL.com" });
            var User = UserController.UpdateUser(55555, new User { Id = 55555, Name = "name4", Password = "qwer3!", Email = "email@EMAIL.com" });

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }

        #endregion


#region  UpdateChange

        [Fact]
        public void PUT_UpdatePassword_UserDoesNotExist_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.UpdatePassword(1, new Password { Old = "Qwerty1!", New = "Qwerty2!" });

            // Assert
            Assert.IsType<NotFoundResult>(User);  
        }

        [Fact]
        public void PUT_UpdatePassword_OldPasswordIsWhiteSpace_NotFound() 
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 3245, Name = "Qwertyu1awer", Password = "Qwertyu1awer1!", Email = "EMAIL@EMAIL.com" });
            var User = UserController.UpdatePassword(3245, new Password { Old = "", New = "Qwertyu1awer22" });

            // Assert
            Assert.IsType<NotFoundResult>(User);  
        }

        [Fact]
        public void PUT_UpdatePassword_NewPasswordIsWhiteSpace_NotFound() 
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 7888, Name = "Qwertyuiop", Password = "Qwerty1!1!", Email = "EMAIL@EMAIL.com" });
            var User = UserController.UpdatePassword(7888, new Password { Old = "a", New = " " });

            // Assert
            Assert.IsType<NotFoundResult>(User);  
        }

        [Fact]
        public void PUT_UpdatePassword_OldPasswordIsWrong_NotFound() 
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 7888, Name = "Qwertyuiopp!1", Password = "Qwertyuiopp!1!", Email = "EMAIL@EMAIL.com" });
            var User = UserController.UpdatePassword(7888, new Password { Old = "Qwertyuiopp!1!f", New = "newPass1234!" });

            // Assert
            Assert.IsType<NotFoundResult>(User);  
        }

        [Fact]
        public void PUT_UpdatePassword_OldPasswordIsOkay_NoContent() 
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.Register(new User { Id = 7888, Name = "Qwertyuiopp", Password = "Qwertyuiopp!1", Email = "EMAIL@EMAIL.com" });
            var User = UserController.UpdatePassword(7888, new Password { Old = "Qwertyuiopp!1", New = "r1Qwertyuiopp!" });

            // Assert
            Assert.IsType<NoContentResult>(User);  
        }


#endregion

        [Fact]
        public void DELETE_Delete_DeleteExistingUser_NoContent()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            UserController.DeleteUsers();
            UserController.Register(new User { Id = 4, Name = "Qwertyuiopp", Password = "Qwertyuiopp1!", Email = "test@test.com" });
            var User = UserController.Delete(4);

            // Assert
            Assert.IsType<NoContentResult>(User);
        }

        [Fact]
        public void DELETE_Delete_DeleteNotExistingUser_NotFound()
        {
            // Arrange
            var UserController = Utils.GetUserController();

            // Act
            var User = UserController.Delete(5);

            // Assert
            Assert.IsType<NotFoundResult>(User);
        }
    }
}
