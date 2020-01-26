using System;
using System.Threading.Tasks;
using Red.CookieSessions;
using Red.CookieSessions.EFCore;
using Red.Validation;
using Validation;

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
            
            server.Get("/api/todos", Auth.IsLoggedIn, Todos.List);
            server.Post("/api/todo", Auth.IsLoggedIn, Valid.NewTodo, Todos.Add);
            server.Delete("/api/todo", Auth.IsLoggedIn, Valid.TodoId, Todos.Remove);
            
            server.Get("/api/login", Valid.Credentials, Auth.Login);
            server.Get("/api/register", Valid.Credentials, Auth.CreateUser);
            server.Get("/api/logout", Auth.IsLoggedIn, Auth.Logout);

            await server.RunAsync();
        }
    }

    public static class Valid
    {
        public static Func<Request, Response, Task<HandlerType>> Credentials = ValidatorBuilder.New()
            .RequiresString("username")
            .RequiresString("password")
            .BuildRedFormMiddleware();

        public static Func<Request, Response, Task<HandlerType>> NewTodo = ValidatorBuilder.New()
            .RequiresString("title")
            .RequiresString("description")
            .BuildRedFormMiddleware();

        public static Func<Request, Response, Task<HandlerType>> TodoId = ValidatorBuilder.New()
            .RequiresInteger("todoId")
            .BuildRedQueryMiddleware();
    }
}