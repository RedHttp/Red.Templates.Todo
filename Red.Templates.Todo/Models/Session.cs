using Red.CookieSessions;

namespace Red.Templates.Todo.Models
{
    public class Session : CookieSessionBase
    {
        public int UserId { get; set; }
    }
}