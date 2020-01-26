using System;
using System.Threading.Tasks;
using Red.Validation;
using Validation;

namespace Red.Templates.Todo
{
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