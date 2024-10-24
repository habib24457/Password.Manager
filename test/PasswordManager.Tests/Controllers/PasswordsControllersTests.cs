using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PasswordManager.Controllers;
using NSubstitute;
using PasswordManager.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PasswordManager.Tests.ControllersTests
{
    
    
    public class PasswordsControllersTests
    {
   
        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfPasswords()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PasswordsDbContext>()
                .UseInMemoryDatabase(databaseName: "passwordsDB")
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
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<List<Password>>(okResult.Value); 
            Assert.Equal(2, returnValue.Count);
        }
    }
}