using System;

namespace Application.Interfaces;

public interface IAiService
{
    public Task<List<string>> GetWordsFromQuestion { get; set; }
}
