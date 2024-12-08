using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace BlogApp.Models
{
    public static class DBHandler
    {
        private static DocumentStore store;

        static DBHandler()
        {
            try
            {
                store = new DocumentStore
                {
                    Urls = new[] { "http://localhost:8080" },
                    Database = "BlogApp"
                };
                store.Initialize();
                Console.WriteLine("Conexiune reușită la RavenDB");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la conectarea la RavenDB: {e.Message}");
            }
        }

        public static async Task<bool> RegisterUser(User u)
        {
            try
            {
                u.Id = Guid.NewGuid().ToString();
                //u.PictureFileName = "UserIcon.jpg";

                using (var session = store.OpenAsyncSession())
                {
                    await session.StoreAsync(u);
                    await session.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la înregistrarea utilizatorului: {e.Message}");
                return false;
            }
        }

        public static async Task<(bool, User)> ValidUser(string email, string password)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    var user = await session.Query<User>()
                        .Where(u => u.Email == email && u.Password == password)
                        .FirstOrDefaultAsync();

                    return (user != null, user);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la verificarea utilizatorului: {e.Message}");
                return (false, null);
            }
        }

        public static async Task<User> GetUser(string email)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    var user = await session.Query<User>()
                        .Where(u => u.Email == email)
                        .FirstOrDefaultAsync();
                    return user;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la obținerea utilizatorului: {e.Message}");
                return null;
            }
        }

        public static async Task<bool> UpdateUser(User u, string newPassword, string filename)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    var user = await session.LoadAsync<User>(u.Id);
                    if (user != null)
                    {
                        user.Password = newPassword;
                        //user.PictureFileName = filename;
                        await session.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la actualizarea utilizatorului: {e.Message}");
                return false;
            }
        }

        public static async Task<bool> DeleteUser(string id)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    session.Delete(id);
                    await session.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la ștergerea utilizatorului: {e.Message}");
                return false;
            }
        }

        public static async Task<bool> AddPost(Post p)
        {
            try
            {
                p.Id = Guid.NewGuid().ToString();
                p.Date = DateTime.Now.ToString("MMMM dd, yyyy");

                using (var session = store.OpenAsyncSession())
                {
                    await session.StoreAsync(p);
                    await session.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la adăugarea postului: {e.Message}");
                return false;
            }
        }

        public static async Task<List<Post>> GetPosts()
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    var posts = await session.Query<Post>().ToListAsync();
                    return posts;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la obținerea posturilor: {e.Message}");
                return new List<Post>();
            }
        }

        public static async Task<bool> UpdatePost(Post p)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    var post = await session.LoadAsync<Post>(p.Id);
                    if (post != null)
                    {
                        post.Title = p.Title;
                        post.Content = p.Content;
                        post.Date = p.Date;
                        await session.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la actualizarea postului: {e.Message}");
                return false;
            }
        }

        public static async Task<bool> DeletePost(string id)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    session.Delete(id);
                    await session.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la ștergerea postului: {e.Message}");
                return false;
            }
        }
        public static async Task<List<User>> GetUsersList()
        {
            using (var session = store.OpenAsyncSession())
            {
                return await session.Query<User>().ToListAsync();
            }
        }

        public static async Task<User> GetUserById(string id)
        {
            using (var session = store.OpenAsyncSession())
            {
                return await session.LoadAsync<User>(id);
            }
        }

        public static async Task<bool> UpdateUserByAdmin(User u)
        {
            try
            {
                using (var session = store.OpenAsyncSession())
                {
                    var user = await session.LoadAsync<User>(u.Id);
                    if (user != null)
                    {
                        user.Username = u.Username;
                        user.Email = u.Email;
                        user.Password = u.Password;
                        await session.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la actualizarea utilizatorului: {e.Message}");
                return false;
            }
        }

        

    }
}
