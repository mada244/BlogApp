using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlogApp.Models
{
    public class Post
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }
        [Required(ErrorMessage = "* Required Field")]
        public string Title { get; set; }
        [Required(ErrorMessage = "* Required Field")]
        public string Content { get; set; }

        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserProfilePicture { get; set; }
        public string Date { get; set; }

    }
}
