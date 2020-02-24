using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Red.CookieSessions;
using Red.Interfaces;
using Red.Templates.Todo.Models;

namespace Red.Templates.Todo.Routers
{
    public static class Authentication
    {
        public static void Bind(IRouter router)
        {
            router.Get("login", Validation.Credentials, Login);
            router.Get("register", Validation.Credentials, CreateUser);
            router.Get("logout", IsLoggedIn, Logout);
        }
        public static async Task<HandlerType> IsLoggedIn(Request req, Response res)
        {
            if (req.GetData<Session>() == null)
            {
                return await res.SendStatus(HttpStatusCode.Unauthorized);
            }
            return HandlerType.Continue;
        }
        private static async Task<HandlerType> Login(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            string username = form["username"];
            string password = form["password"];
            
            await using var ctx = new TodoContext(0);

            var user = await ctx.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            await res.OpenSession(new Session { UserId = user.Id });
            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> Logout(Request req, Response res)
        {
            var session = req.GetData<Session>();
            await res.CloseSession(session);
            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> CreateUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            
            var user = new User
            {
                Username = form["username"],
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(form["password"])
            };
            
            await using var ctx = new TodoContext(0);
            var existing = await ctx.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existing != null)
            {
                return await res.SendString("Username taken", status: HttpStatusCode.BadRequest);
            }

            ctx.Add(user);
            await ctx.SaveChangesAsync();

            return await res.SendStatus(HttpStatusCode.OK);
        }
    }
}