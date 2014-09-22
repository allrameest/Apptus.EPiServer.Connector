using System.Web;

namespace Apptus.ESales.EPiServer.Util
{
    public static class Session
    {
        public static string Key
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session != null)
                    {
                        return HttpContext.Current.Session.SessionID;
                    }

                    HttpCookie cookie = HttpContext.Current.Request.Cookies["ASP.NET_SessionId"];
                    if (cookie != null)
                    {
                        return cookie.Value;
                    }
                }

                return string.Empty;
            }
        }
    }
}
