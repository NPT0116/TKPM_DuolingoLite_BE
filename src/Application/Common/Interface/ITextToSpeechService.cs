using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using SharedKernel;

namespace Application.Common.Interface
{
    public interface ITextToSpeechService
    {
        byte[] GenerateAudioFileFromText(string text);
        Task<Result<Media>> GenerateMediaFromText(string text, string? folder);
    }
}