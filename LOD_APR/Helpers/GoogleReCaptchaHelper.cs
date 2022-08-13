using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LOD_APR.Helpers
{
    public class GoogleReCaptchaHelper
    {
        public bool IsCaptchaValid(string gRecaptchaResponse)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string secretKey = Convert.ToString(ConfigurationManager.AppSettings.Get("InternalCaptchaSecret"));
                string webCaptcha = Convert.ToString(ConfigurationManager.AppSettings.Get("webCaptcha"));
                decimal requirement = Convert.ToDecimal(ConfigurationManager.AppSettings.Get("requirementCaptcha"));
                var res = httpClient.GetAsync(string.Format(webCaptcha, secretKey, gRecaptchaResponse)).Result;

                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                string JSONres = res.Content.ReadAsStringAsync().Result;
                dynamic JSONdata = JObject.Parse(JSONres);

                var recaptchaScore = requirement; // nivel fijado para exigencia del captcha. 

                if (JSONdata.success != "true" || JSONdata.score <= recaptchaScore)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}