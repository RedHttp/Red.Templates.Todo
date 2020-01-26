using Red.CookieSessions;

namespace Red.Templates.Todo
{
    public class Session : CookieSessionBase
    {
        public int UserId { get; set; }
    }
}