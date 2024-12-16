using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlogApp.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")] 
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        [Required(ErrorMessage = "* Required Field")]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required(ErrorMessage = "* Required Field")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* Required Field")]
        [MinLength(4)]
        public string Password { get; set; }

        //[JsonProperty(PropertyName = "pictureFileName")]
        //public string PictureFileName { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; } = "User";
    }
}
