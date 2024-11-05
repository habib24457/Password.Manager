using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Xunit;
using PasswordManager.Controllers;
using NSubstitute;
using PasswordManager.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Service;

namespace PasswordManager.Tests.ControllersTests
{
    public class PasswordsControllersTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfPasswords()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PasswordsDbContext>()
                .UseInMemoryDatabase(databaseName: $"passwordsDB_{Guid.NewGuid()}")
                .Options;
            var context = new PasswordsDbContext(options);
            context.PasswordItem.AddRange(new List<Password>
            {
                new Password { Id = 1, UserName = "user1", Category = "Social", App = "Facebook", UserPassword = "encryptedPass1" },
                new Password { Id = 2, UserName = "user2", Category = "Work", App = "Slack", UserPassword = "encryptedPass2" }
            });
            await context.SaveChangesAsync();
            var controller = new PasswordsController(context);

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<List<Password>>(okResult.Value); 
            Assert.Equal(2, returnValue.Count);
        }
        
        [Fact]
        public async Task GetAll_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PasswordsDbContext>()
                .UseInMemoryDatabase(databaseName: $"passwordsDB_{Guid.NewGuid()}")
                .Options;
            var controller = new PasswordsController(new PasswordsDbContext(options)); // No passwords added to the context

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Password>>(okResult.Value);
            Assert.Empty(returnValue);
        }
        
        [Fact]
        public async Task EncryptionService_ShouldReturn_EncryptedAndDecryptedPassword()
        {
            // Arrange
            var newPasswordItem = new Password
            {
                UserName = "user1",
                Category = "Social",
                App = "Facebook",
                UserPassword ="encryptedPass1"
            };
            var encryptionService = new EncryptionService();

            // Act
            var encryptedPass = encryptionService.EncryptPassword(newPasswordItem.UserPassword);
            var decryptedPass = encryptionService.DecryptPassword(encryptedPass);

            //Assert
            Assert.NotEqual(newPasswordItem.UserPassword,encryptedPass);
            Assert.Equal(newPasswordItem.UserPassword,decryptedPass);
        }
        
    }
}