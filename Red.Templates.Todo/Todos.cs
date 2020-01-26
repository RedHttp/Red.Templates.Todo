using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Red.Templates.Todo
{
    public static class Todos
    {
        public static async Task<HandlerType> List(Request req, Response res)
        {
            var session = req.GetData<Session>();
            
            await using var ctx = new TodoContext(session.UserId);
            var todos = await ctx.Todos.ToListAsync();

            return await res.SendJson(todos);
        }
        public static async Task<HandlerType> Add(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var session = req.GetData<Session>();

            var todo = new Todo
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
        public static async Task<HandlerType> Remove(Request req, Response res)
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