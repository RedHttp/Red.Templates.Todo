using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Red.Interfaces;
using Red.Templates.Todo.Models;

namespace Red.Templates.Todo.Routers
{
    public static class Todos
    {
        public static void Bind(IRouter router)
        {
            router.Get("/list", Authentication.IsLoggedIn, List);
            router.Post("", Authentication.IsLoggedIn, Validation.NewTodo, Add);
            router.Delete("", Authentication.IsLoggedIn, Validation.TodoId, Remove);
        }
        
        private static async Task<HandlerType> List(Request req, Response res)
        {
            var session = req.GetData<Session>();
            
            await using var ctx = new TodoContext(session.UserId);
            var todos = await ctx.Todos.ToListAsync();

            return await res.SendJson(todos);
        }
        private static async Task<HandlerType> Add(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var session = req.GetData<Session>();

            var todo = new Models.Todo
            {
                UserId = session.UserId,
                Title = form["title"],
                Description = form["description"],
                Created = DateTime.UtcNow
            };
            
            await using var ctx = new TodoContext(session.UserId);
            ctx.Add(todo);
            await ctx.SaveChangesAsync();

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> Remove(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var session = req.GetData<Session>();
            var todoId = int.Parse(form["todoId"]);
            
            await using var ctx = new TodoContext(session.UserId);
            var existing = await ctx.Todos.FirstOrDefaultAsync(todo => todo.Id == todoId);
            if (existing == null)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            ctx.Remove(existing);
            await ctx.SaveChangesAsync();

            return await res.SendStatus(HttpStatusCode.OK);
        }
    }
}