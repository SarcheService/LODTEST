using LOD_APR.Areas.GLOD.ModelsViews;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    public static class Mop_API
    {
        public static async Task<MOP_Post_Response> API_postListSMS(MOP_Post_Sign mop_sign)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string ruta_api = ConfigurationManager.AppSettings.Get("mop_api_url").ToString();

                    MOP_Post_Response response = new MOP_Post_Response();
                    client.BaseAddress = new Uri(ruta_api);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("OTP", mop_sign.otp);

                    var postTask = client.PostAsJsonAsync<MOP_Post_Sign>("files/tickets", mop_sign);
                    postTask.Wait();

                    var result = postTask.Result;
                    string responseBody = await result.Content.ReadAsStringAsync();
                    MOP_Post_Response resultado = JsonConvert.DeserializeObject<MOP_Post_Response>(responseBody);
                    if (resultado.files != null)
                    {
                        if (resultado.files[0].status != "OK")
                            resultado.error = resultado.files[0].status;
                    }
                    return resultado;
                }
                catch (Exception ex)
                {
                    return new MOP_Post_Response() { error = ex.Message };
                }
            }
        }
    }
}