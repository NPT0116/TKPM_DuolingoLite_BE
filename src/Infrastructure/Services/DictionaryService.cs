using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Features.Learning.Words.Queries.GetWordDefinition;
using Domain.Entities.Learning.Words;
using Google.Apis.Auth.OAuth2.Requests;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly HttpClient _httpClient;
        private string GetRequestUrl(string word) => $"api/v2/entries/en/{word}";
        public DictionaryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WordDefinitionDto>> GetWordDefinition(string word)
        {
            var requestUrl = GetRequestUrl(word);

            var response = await _httpClient.GetAsync(requestUrl);
            try
            {
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();

                // Adjust the deserialization settings if needed (e.g., property naming policy)
                var entries = JsonConvert.DeserializeObject<List<WordDefinitionDto>>(jsonContent);
                return entries;
                
            }
            catch (System.Exception ex)
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                string errorJson = JsonConvert.SerializeObject(ex, settings);
                Console.WriteLine(errorJson);
                return new List<WordDefinitionDto>();
            }
        }
    }
}