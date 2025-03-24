using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Learning.Questions.Options;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Options.Commands.CreateOption
{
    public class CreateOptionCommandHandler : ICommandHandler<CreateOptionCommand, Option>
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        public CreateOptionCommandHandler(
            IOptionRepository optionRepository,
            IMediaRepository mediaRepository,
            ITextToSpeechService textToSpeechService,
            IMediator mediator,
            IApplicationDbContext context)
        {
            _optionRepository = optionRepository;
            _mediaRepository = mediaRepository;
            _textToSpeechService = textToSpeechService;
            _mediator = mediator;
            _context = context;
        }

        public async Task<Result<Option>> Handle(CreateOptionCommand request, CancellationToken cancellationToken)
        {
            var (vietnamese, image, audio, english, needAudio) = request.dto;
            var similarOption = await _optionRepository.FindOption(english, vietnamese);
            if (similarOption != null)
            {
                return Result.Failure<Option>(OptionError.OptionAlreadyExists());
            }

            Domain.Entities.Media.Media? optionImage = null;

            if(image != null)
            {
                var createImage = await _mediaRepository.UploadFileAsync(
                    image, image, Domain.Entities.Media.Enums.MediaType.Image, 
                    10, DateTime.UtcNow, DateTime.UtcNow, image, cancellationToken);

                if(createImage.IsFailure) return Result.Failure<Option>(createImage.Error);
                optionImage = createImage.Value;
            }

            Domain.Entities.Media.Media? optionAudio = null;
            if(audio != null)
            {
                var createAudio = await _mediaRepository.UploadFileAsync(
                    audio, audio, Domain.Entities.Media.Enums.MediaType.Audio, 
                    10, DateTime.UtcNow, DateTime.UtcNow, audio, cancellationToken);

                if(createAudio.IsFailure) return Result.Failure<Option>(createAudio.Error);
                optionAudio = createAudio.Value;
            }
            else
            {
                if(needAudio)
                {
                    if(english == null) return Result.Failure<Option>(OptionError.OptionAudioRequiresEnglishText());
                    byte[] audioBytes = _textToSpeechService.GenerateAudioFileFromText(english);
                    var uploadRequest = new MediaUploadRequest(
                        string.Empty,
                        audioBytes,
                        english,
                        "audio/mp3"
                    );
                    var uploadCommand = new MediaUploadCommand(uploadRequest);
                    var uploadedFile = await _mediator.Send(uploadCommand);
                    if(uploadedFile.IsFailure) return Result.Failure<Option>(uploadedFile.Error);
                    optionAudio = uploadedFile.Value;
                }
            }

            var createOption = Option.Create(vietnamese, optionImage, optionAudio, english);
            if(createOption.IsFailure) return Result.Failure<Option>(createOption.Error);
            var option = createOption.Value;
            await _optionRepository.CreateOption(option);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Result.Success(option);
        }
    }
}