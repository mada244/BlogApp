using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Models
{
    static public class UserRepository
    {
        public static List<User> users = new List<User>();
        static UserRepository()
        {
        }
    }
}
