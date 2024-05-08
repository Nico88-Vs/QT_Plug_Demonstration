using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Test_Api_Connection
{
    public class Connections
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public static readonly string postwithbody = "http://localhost:5000/sample?message=New%20Value";
        public static readonly string req_type = "http://localhost:5000/sample/get-message";
        public event EventHandler<string> message_recived;

        public virtual void oN_Message_Recived(string message)
        {
            message_recived?.Invoke(this, message);
        }

        public void StartGetPats(string url)
        {
            if (url != req_type)
                Task.Run(async () => await Get_Pats(url));

            else
            {
                Task.Run(async () =>
                {
                    while (true) // Assicurati di avere una condizione di arresto o questo verrà eseguito all'infinito.
                    {
                        await Get_Pats(url);
                        await Task.Delay(250); // Pausa di 0.25 secondi prima della prossima esecuzione
                    }
                });
            }
        }

        public async Task Get_Pats(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();
                oN_Message_Recived(responseBody);
            }
            catch (HttpRequestException e)
            {
                oN_Message_Recived($"Errore di  {e}");
            }
            catch (Exception e)
            {
                oN_Message_Recived($"Errore sconosciuto {e}");
            }

        }
    }

}
