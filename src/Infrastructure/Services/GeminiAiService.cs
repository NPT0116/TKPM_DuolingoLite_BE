using System;
using System.Threading.Tasks;
using Application.Common.Utls;
using Application.Interfaces;
using Infrastructure.Config;
using Models.Enums;
using Newtonsoft.Json;

namespace Infrastructure.Services;

public class GeminiAiService : IAiService
{
    private readonly GeminiConfig _config;
    public GeminiAiService(GeminiConfig config)
    {
        _config = config;
    }

    public async Task<List<string>> SplitWordsFromString(string prompt)
    {
        var knowledBase = @"Please split the input sentence into an array of strings, each being a meaningful word or phrase. Keep phrasal verbs (e.g. ""wake up"") and multi-word expressions (e.g. ""table tennis"", ""a lot of"") together. However, treat articles (""a"", ""an"", ""the"", ""these"", ""there"") as separate elements even if they adjoin other words. Also, any punctuation should stay attached to the word immediately before it.
Examples:
""I have a lot of money."" → [""I"", ""have"", ""a"", ""lot of"", ""money.""]
""I wake up at 7 am."" → [""I"", ""wake up"", ""at"", ""7"", ""am.""]
""how long does it take to regain a heart in duolingo?"" → [""how long"", ""does"", ""it"", ""take"", ""to"", ""regain"", ""a"", ""heart"", ""in"", ""duolingo?""]
""I have been using these websites since I was a freshman at university."" → [""I"", ""have"", ""been"", ""using"", ""these"", ""websites"", ""since"", ""I"", ""was"", ""a"", ""freshman"", ""at"", ""university.""]
Apply these rules to every sentence.";
        var service = _config.BuildApiRequest(prompt, knowledBase);
        var genetor = _config.CreateGenerator();
        var model = ModelVersion.Gemini_20_Flash_Lite;
        var response = await genetor.GenerateContentAsync(service, model);
        PrintUtils.PrintAsJson(response);
        return ParseWordsFromString(response.Result);
    }
public List<string> ParseWordsFromString(string input)
{
    try
    {
        // Deserialize chuỗi JSON thành List<string>
        var words = JsonConvert.DeserializeObject<List<string>>(input);
        return words ?? new List<string>();
    }
    catch (Exception ex)
    {
        // Nếu xảy ra lỗi, fallback sang tách theo khoảng trắng
        return input.Split(" ").ToList();
    }
}
}
