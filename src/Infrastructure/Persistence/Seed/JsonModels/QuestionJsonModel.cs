using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    public class QuestionJsonModel
{
    public string Instruction { get; set; }
    public string VietnameseText { get; set; }
    public string EnglishText { get; set; }
    public string Type { get; set; }
    public string Image { get; set; }
    public string Audio { get; set; }
    public List<WordJsonModel> Words { get; set; }
    public ConfigurationJsonModel QuestionConfiguration { get; set; }
    public ConfigurationJsonModel OptionConfiguration { get; set; }
    public int Order { get; set; }
    public List<OptionJsonModel> Options { get; set; } = new();
}

}