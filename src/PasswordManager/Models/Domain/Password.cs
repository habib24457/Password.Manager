using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordManager.Models.Domain
{
    public class Password
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes the auto increment
        public int Id { get; set; }
        public string Category { get; set; }
        public string App { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}

