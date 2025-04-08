using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Interface;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Media.Constants;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Options.Commands.UpdateOption
{
    public class UpdateOptionCommandHandler : ICommandHandler<UpdateOptionCommand, Option>
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IMediaStorageService _storageService;
        private readonly IMediaRepository _mediaRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly IApplicationDbContext _context;
        private readonly List<string> UploadedFiles = new List<string>();
        private readonly List<string> FilesToDelete = new List<string>();
        public UpdateOptionCommandHandler(
            IOptionRepository optionRepository,
            IMediaStorageService mediaStorageService,
            IMediaRepository mediaRepository,
            IQuestionRepository questionRepository,
            IApplicationDbContext context,
            ITextToSpeechService textToSpeechService)
        {
            _optionRepository = optionRepository;
            _storageService = mediaStorageService;
            _mediaRepository = mediaRepository;
            _questionRepository = questionRepository;
            _context = context;
            _textToSpeechService = textToSpeechService;
        }
        public async Task<Result<Option>> Handle(UpdateOptionCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            var result = await HandleUpdateOption(request, cancellationToken);
            if(result.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                foreach(var file in UploadedFiles)
                {
                    await _storageService.DeleteFileFromUrl(file);
                }
                return Result.Failure<Option>(result.Error);
            }

            foreach(var file in FilesToDelete)
            {
                await _storageService.DeleteFileAsync(file, cancellationToken);
            }
            
            await transaction.CommitAsync(cancellationToken);
            return Result.Success(result.Value);            

        }

        private async Task<Result<Option>> HandleUpdateOption(UpdateOptionCommand request, CancellationToken cancellationToken)
        {
            var (optionId, optionDto) = request;
            var option = await _optionRepository.GetOptionById(optionId);
            if(option == null)
            {
                return Result.Failure<Option>(OptionError.OptionNotFound);
            }

            var (vietnameseText, imageUrl, audioUrl, englishText, isAudioGenerated, isImageGenerated) = optionDto;
            var questions = await _questionRepository.GetQuestionsThatUseOption(optionId);
            
            if(string.IsNullOrEmpty(vietnameseText))
            {
                var isVietnameseTextUsed = questions.Any(q => q.QuestionConfiguration.VietnameseText);
                if(isVietnameseTextUsed)
                {
                    return Result.Failure<Option>(OptionError.VietnameseTextInUsed);
                }
            }

            if(string.IsNullOrEmpty(englishText))
            {
                var isEnglishTextUsed = questions.Any(q => q.QuestionConfiguration.EnglishText);
                if(isEnglishTextUsed)
                {
                    return Result.Failure<Option>(OptionError.EnglishTextInUsed);
                }
            }

            Domain.Entities.Media.Media? newAudio = null;
            var getAudio = await GetNewAudio(optionDto, option, questions, cancellationToken);
            if(getAudio.IsFailure) return Result.Failure<Option>(getAudio.Error);
            newAudio = getAudio.Value;
            
            Domain.Entities.Media.Media? newImage = null;
            var getImage = await GetNewImage(optionDto, option, questions, cancellationToken);
            if(getImage.IsFailure) return Result.Failure<Option>(getImage.Error);
            newImage = getImage.Value;
            
            option.Update(vietnameseText, newImage, newAudio, englishText);

            return Result.Success<Option>(option);
        }
        private async Task<Result<Domain.Entities.Media.Media>> UploadNewAudio(string audioUrl, CancellationToken cancellationToken)
        {
            return await _mediaRepository.UploadFileAsync(
                audioUrl,
                audioUrl,
                MediaType.Audio,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow,
                audioUrl,
                cancellationToken
            );
        }

        private async Task<Result<Domain.Entities.Media.Media?>> GetNewAudio(
            UpdateOptionDto dto, 
            Option option,
            List<Domain.Entities.Learning.Questions.Question> questions,
            CancellationToken cancellationToken 
            )
        {
            Domain.Entities.Media.Media? newAudio = null;
            if(dto.isAudioGenerated)
            {
                if(string.IsNullOrEmpty(dto.englishText))
                {
                    return Result.Failure<Domain.Entities.Media.Media?>(OptionError.OptionAudioRequiresEnglishText());
                }

                var createAudio = await _textToSpeechService.GenerateMediaFromText(dto.englishText, MediaConstants.Audio);
                if(createAudio.IsFailure) return Result.Failure<Domain.Entities.Media.Media?>(createAudio.Error);
                UploadedFiles.Add(createAudio.Value.Url);

                if(option.Audio != null)
                {
                    FilesToDelete.Add(option.Audio.Url);
                    await _mediaRepository.DeleteFile(option.Audio);
                }
                
                newAudio = createAudio.Value;
                
            }
            else
            {
                if(string.IsNullOrEmpty(dto.audioUrl))
                {
                    var isAudioUsed = questions.Any(q => q.QuestionConfiguration.Audio);
                    if(isAudioUsed)
                    {
                        return Result.Failure<Domain.Entities.Media.Media?>(OptionError.AudioInUsed);
                    }
                }
                else
                {
                    if(option.Audio != null)
                    {
                        if(dto.audioUrl != option.Audio.Url)
                        {
                            var newMedia = await UploadNewAudio(dto.audioUrl, cancellationToken);
                            if(newMedia.IsFailure) return Result.Failure<Domain.Entities.Media.Media?>(newMedia.Error);
                            newAudio = newMedia.Value;

                            FilesToDelete.Add(option.Audio.Url);
                            await _mediaRepository.DeleteFile(option.Audio);
                        }
                        else
                        {
                            newAudio = option.Audio;
                        }
                    }
                    else
                    {
                        var newMedia = await UploadNewAudio(dto.audioUrl, cancellationToken);

                        if(newMedia.IsFailure) return Result.Failure<Domain.Entities.Media.Media?>(newMedia.Error);
                        newAudio = newMedia.Value;
                    }
                }    
            }

            return Result.Success(newAudio);
        }
    
        private async Task<Result<Domain.Entities.Media.Media?>> GetNewImage(
            UpdateOptionDto dto,
            Option option,
            List<Domain.Entities.Learning.Questions.Question> questions,
            CancellationToken cancellationToken
            )
        {
            Domain.Entities.Media.Media? newImage = null;
            if(string.IsNullOrEmpty(dto.imageUrl))
            {
                var isImageUsed = questions.Any(q => q.QuestionConfiguration.Image);
                if(isImageUsed)
                {
                    return Result.Failure<Domain.Entities.Media.Media?>(OptionError.ImageInUsed);
                }

                if(option.Image != null)
                {
                    FilesToDelete.Add(option.Image.Url);
                    await _mediaRepository.DeleteFile(option.Image);
                }
            }
            else
            {
                if(option.Image != null)
                {
                    if(dto.imageUrl != option.Image.Url)
                    {   
                        var newMedia = await _mediaRepository.UploadFileAsync(
                            dto.imageUrl,
                            dto.imageUrl,
                            MediaType.Image,
                            10,
                            DateTime.UtcNow,
                            DateTime.UtcNow,
                            dto.imageUrl,
                            cancellationToken
                        );

                        if(newMedia.IsFailure) return Result.Failure<Domain.Entities.Media.Media?>(newMedia.Error);
                        newImage = newMedia.Value;

                        FilesToDelete.Add(option.Image.Url);
                        await _mediaRepository.DeleteFile(option.Image);
                    }
                    else
                    {
                        newImage = option.Image;
                    }
                }
                else
                {
                    var newMedia = await _mediaRepository.UploadFileAsync(
                        dto.imageUrl,
                        dto.imageUrl,
                        MediaType.Image,
                        10,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        dto.imageUrl,
                        cancellationToken
                    );

                    if(newMedia.IsFailure) return Result.Failure<Domain.Entities.Media.Media?>(newMedia.Error);
                    newImage = newMedia.Value;
                }

            }

            return Result.Success(newImage);
        }
    }
}