using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Models
{
    static public class PostRepository
    {
        public static List<Post> posts = new List<Post>();
        static PostRepository()
        {
        }
    }
}
