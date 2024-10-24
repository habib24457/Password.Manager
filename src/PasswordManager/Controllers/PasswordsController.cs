using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Models.Domain;
using PasswordManager.Service;

namespace PasswordManager.Controllers
{
    // /api/passwords
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        private readonly PasswordsDbContext _dbContext;
        private EncryptionService encryptPasswordObj = new EncryptionService();

        public PasswordsController(PasswordsDbContext passwordsDbContext)
        {
            _dbContext = passwordsDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var passwords = await _dbContext.PasswordItem.ToListAsync();
            return Ok(passwords);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword([FromBody] Password passwordItem)
        {
            if (passwordItem == null)
                return BadRequest();

            var encryptedPassword = encryptPasswordObj.EncryptPassword(passwordItem.UserPassword);
            var addPassword = new Password
            {
                UserName = passwordItem.UserName,
                Category = passwordItem.Category,
                App = passwordItem.App,
                UserPassword = encryptedPassword
            };
            _dbContext.Add(addPassword);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction("CreatePassword", new { id = passwordItem.Id }, passwordItem);
        }

        //Get One Password decrypted or encrypted
        [HttpGet]
        [Route("{id:int}/{isDecrypted}")]
        public async Task<IActionResult> GetOneById([FromRoute] int id, bool isDecrypted)
        {
            if (id == null)
                return BadRequest();

            var passwordItem = await _dbContext.PasswordItem.FindAsync(id);

            if (passwordItem == null)
            {
                return NotFound();
            }

            if (isDecrypted)
            {
                var encryptedPassword = passwordItem.UserPassword;
                var decryptedPassword = encryptPasswordObj.DecryptPassword(encryptedPassword);
                passwordItem.UserPassword = decryptedPassword;
                return Ok(passwordItem);
            }

            return Ok(passwordItem);
        }

        //Update a Password store item
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdatePassword([FromBody] Password password, [FromRoute] int id)
        {
            if (id == null)
                return BadRequest();
            var existingPassword = await _dbContext.PasswordItem.FindAsync(id);
            if (existingPassword == null)
            {
                return NotFound();
            }

            existingPassword.UserName = password.UserName;
            existingPassword.App = password.App;
            existingPassword.Category = password.Category;
            existingPassword.UserPassword = encryptPasswordObj.EncryptPassword(password.UserPassword);
            await _dbContext.SaveChangesAsync();

            return Ok(existingPassword);
        }

        //Delete a Password store item
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeletePassword([FromRoute] int id)
        {
            if (id == null)
                return BadRequest();

            var existingPassword = await _dbContext.PasswordItem.FindAsync(id);
            if (existingPassword == null)
                return NotFound();
            _dbContext.PasswordItem.Remove(existingPassword);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

