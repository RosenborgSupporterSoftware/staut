using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RBKweb.Integration
{
    /// <summary>
    /// En klasse som gjør drittjobben med å prate http mot RBKweb-forumet
    /// </summary>
    public class RBKwebConnection
    {
        #region Fields

        private readonly string _username;
        private readonly string _password;
        private HttpClientHandler _handler;
        private CookieContainer _cookies;

        private Uri pageUri;
        private bool _loggedIn = false;

        #endregion

        public RBKwebConnection(string username, string password)
        {
            _username = username;
            _password = password;

            _handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                //CookieContainer = new CookieContainer()
            };

            _cookies = new CookieContainer();
        }

        /// <summary>
        /// Logger inn på RBKweb med brukernavn/passord oppgitt
        /// </summary>
        /// <returns>true hvis vi fikk logget inn, false ellers</returns>
        public async Task<bool> Login()
        {
            var client = new HttpClient(_handler);

            try
            {
                var content = new FormUrlEncodedContent(CreateLoginContent());
                var response =
                    await client.PostAsync("http://rbkweb.no/forum/login.php", content);

                //response.EnsureSuccessStatusCode();

                // TODO: Sjekk om vi faktisk ble logget inn. :)
                _loggedIn = true;

                pageUri = response.RequestMessage.RequestUri;

                IEnumerable<string> cookies;
                if (response.Headers.TryGetValues("Set-Cookie", out cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        var val = cookie.Substring(0, cookie.IndexOf(';'));
                        _cookies.SetCookies(pageUri, val);
                    }
                }

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }

        public async Task<bool> SendMessage(string username, string subject, string message)
        {
            if (!_loggedIn)
                return false;

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("addbbcode18", "#000000"),
                new KeyValuePair<string, string>("addbbcode20", "12"),
                new KeyValuePair<string, string>("helpbox", "meh"),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("attach_sig", "off"),
                new KeyValuePair<string, string>("folder", "inbox"),
                new KeyValuePair<string, string>("mode", "post"),
                new KeyValuePair<string, string>("post", "Submit")
            };

            try
            {
                var baseAddress = new Uri("http://rbkweb.no");
                using (var handler = new HttpClientHandler { UseCookies = false })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var msg = new HttpRequestMessage(HttpMethod.Post, "/forum/privmsg.php");
                    
                    var cookieVal = _cookies.GetCookieHeader(pageUri);
                    msg.Headers.Add("Cookie", cookieVal);
                    
                    msg.Content = new FormUrlEncodedContent(formData);
                    
                    var result = await client.SendAsync(msg);

                    return result.IsSuccessStatusCode;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }

        private List<KeyValuePair<string, string>> CreateLoginContent()
        {
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("username", _username));
            values.Add(new KeyValuePair<string, string>("password", _password));
            values.Add(new KeyValuePair<string, string>("redirect", String.Empty));
            values.Add(new KeyValuePair<string, string>("login", "Log in"));

            return values;
        }
    }
}
