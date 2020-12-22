using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
       [Required]
        public string Username {get; set;}
       [Required]
     //  [Phone][EmailAddress][RegularExpression][StringLength]
        public string Password {get; set;}
        
    }
}