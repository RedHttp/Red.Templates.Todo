using System;
using System.Threading.Tasks;
using Red.CookieSessions;
using Red.CookieSessions.EFCore;
using Red.Templates.Todo.Models;
using Red.Templates.Todo.Routers;

namespace Red.Templates.Todo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new RedHttpServer();

            server.Use(new CookieSessions<Session>(TimeSpan.FromDays(14))
            {
                Store = new EntityFrameworkSessionStore<Session>(() => new TodoContext(0)) 
            });

            server.CreateRouter("/api/auth", Authentication.Bind);
            server.CreateRouter("/api/todo", Todos.Bind);

            await server.RunAsync();
        }
    }
}