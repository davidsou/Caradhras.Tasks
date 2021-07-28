using Caradhras.Tasks.Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Repository
{

    public class CaradhrasRepository : ICaradhrasRepository
    {

        private static HttpClient _client = new HttpClient();
        private readonly AppSettings _appSettings;



        public CaradhrasRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public AutenticacaoCaradhras AutenticacaoCaradhras()
        {
            try
            {
                var result = new AutenticacaoCaradhras();
                var user = new AuthUser();
                user.Username = _appSettings.SafeBoxUser;
                user.Password = _appSettings.SafeBoxInfo;
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = _client.PostAsync(_appSettings.UrlSafeBox, content).Result;
                var stringcontent = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<AutenticacaoCaradhras>(stringcontent);
                return result;
            }
            catch (Exception ex)
            {


                throw new Exception($"Erro na autenticação com o Caradhras: {ex}");
            }
        }

        public async Task<ResultSimple<Tresult>> ConsumeServicesGET<Tresult>(string service, string token)
        {
            var config = new JsonSerializerSettings();
            config.NullValueHandling = NullValueHandling.Ignore;


            var result = new ResultSimple<Tresult>();
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            var response = await _client.GetAsync(service);
            result.Content = await response.Content.ReadAsStringAsync();
            result.Sucess = response.IsSuccessStatusCode;
            result.Data = JsonConvert.DeserializeObject<Tresult>(result.Content, config);

            return result;

        }


        public async Task<ResultSimple<Tresult>> ConsumeServicesGETBureau<Tresult>(string service)
        {

            var result = new ResultSimple<Tresult>();
            var response = await _client.GetAsync(service);
            result.Content = await response.Content.ReadAsStringAsync();
            result.Sucess = response.IsSuccessStatusCode;
            if (result.Sucess)
            {
                var format = "dd/MM/yyyy";
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                result.Data = JsonConvert.DeserializeObject<Tresult>(result.Content, dateTimeConverter);
            }
            return result;

        }

        public async Task<ResultSimple<Tresult>> ConsumeServicesPOST<Tresult, Trequest>(Trequest payload, string service, string token)
        {

            var result = new ResultSimple<Tresult>();
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            }
            var response = await _client.PostAsync(service, content);

            result.Content = await response.Content.ReadAsStringAsync();
            result.Sucess = response.IsSuccessStatusCode;
            if (result.Sucess)
            {
                result.Data = JsonConvert.DeserializeObject<Tresult>(result.Content);
            }

            return result;

        }

        public async Task<ResultSimple<Tresult>> ConsumeServicesPOST<Tresult>(string service, string token)
        {

            var result = new ResultSimple<Tresult>();
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            var response = await _client.PostAsync(service, null);
            result.Content = await response.Content.ReadAsStringAsync();
            result.Sucess = response.IsSuccessStatusCode;
            if (result.Sucess)
                result.Data = JsonConvert.DeserializeObject<Tresult>(result.Content);

            return result;

        }

        public async Task<ResultSimple<Tresult>> ConsumeServicesPUT<Tresult, Trequest>(Trequest payload, string service, string token)
        {

            var result = new ResultSimple<Tresult>();
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            var response = await _client.PutAsync(service, content);

            result.Content = await response.Content.ReadAsStringAsync();
            result.Sucess = response.IsSuccessStatusCode;
            if (result.Sucess)
                result.Data = JsonConvert.DeserializeObject<Tresult>(result.Content);

            return result;

        }




        public async Task<bool> ConsumeServicesPOST(string service, string token)
        {
            bool result;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            var response = await _client.PostAsync(service, null);
            result = response.IsSuccessStatusCode;
            return result;
        }





    }

}
