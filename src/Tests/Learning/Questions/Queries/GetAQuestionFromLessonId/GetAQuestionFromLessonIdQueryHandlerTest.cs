using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Learning.Question.Queries.GetAQuestionFromLessionId;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.Words;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using SharedKernel;
using Xunit;

namespace Tests.Learning.Questions.Queries.GetAQuestionFromLessonId
{
    public class GetAQuestionFromLessonIdQueryHandlerTest
    {
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly GetAQuestionFromLessonIdQueryHandler _handler;

        public GetAQuestionFromLessonIdQueryHandlerTest()
        {
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _handler = new GetAQuestionFromLessonIdQueryHandler(_lessonRepositoryMock.Object);
        }

        #region 1) Lesson not found
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_LessonNotFound()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var query = new GetAQuestionFromLessonIdQuery(lessonId,  1);

            // Mock => returns null => not found
            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync((Lesson)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(LessonError.LessonNotFound);
        }
        #endregion

        #region 2) Question not found
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_QuestionNotFound()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            // We ask for question order=99, which won't exist
            var query = new GetAQuestionFromLessonIdQuery(lessonId,  99);

            // A lesson with no question that has order=99
            var fakeLesson = new Lesson("Test Lesson", xpEarned: 0, order: 1);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(QuestionError.QuestionNotFound);
        }
        #endregion

        #region 3) MultipleChoice => success
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_MultipleChoice()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 1;
            var fakeLesson = new Lesson("Lesson MC", xpEarned: 10, order: 1);

            // Create the needed Configurations via factory
            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            // 1) Create Question
            var qResult = Question.Create(
                instruction: "Choose the correct answer",
                vietnameseText: null,
                audio: null,
                englishText: "Hello?",
                image: null,
                type: QuestionType.MultipleChoice,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            qResult.IsSuccess.Should().BeTrue();
            var question = qResult.Value;

            // 2) Create domain Option
            var opt1 = Option.Create("Xin chào", null, null, "Hello").Value;
            var opt2 = Option.Create("Tạm biệt", null, null, "Bye").Value;

            // 3) Create MC questionOptions
            var mc1 = MultipleChoiceQuestionOption.Create(question, opt1, isCorrect: true, order: 1).Value;
            var mc2 = MultipleChoiceQuestionOption.Create(question, opt2, isCorrect: false, order: 2).Value;

            question.AddOption(mc1);
            question.AddOption(mc2);
            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Type.Should().Be("MultipleChoice");
            dto.Options.Should().HaveCount(2);

            var first = dto.Options[0] as MultipleChoiceOptionDto;
            first.Should().NotBeNull();
            first!.IsCorrect.Should().BeTrue();
            first.VietnameseText.Should().Be("Xin chào");
        }
        #endregion

        #region 4) Matching => success
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_Matching()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 2;
            var fakeLesson = new Lesson("Lesson Matching", xpEarned: 10, order: 1);

            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            var qResult = Question.Create(
                instruction: "Match the pairs",
                vietnameseText: "Ghép cặp",
                audio: null,
                englishText: null,
                image: null,
                type: QuestionType.Matching,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            qResult.IsSuccess.Should().BeTrue();
            var question = qResult.Value;

            var optRes = Option.Create("Con mèo", null, null, "Cat");
            var opt = optRes.Value;

            var matchRes = MatchingQuestionOption.Create(
                question,
                opt,
                MatchingQuestionOptionType.VietnameseText,
                MatchingQuestionOptionType.EnglishText,
                order: 1
            );
            var matchOpt = matchRes.Value;

            question.AddOption(matchOpt);
            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Type.Should().Be("Matching");
            dto.Options.Should().HaveCount(1);

            var mo = dto.Options[0] as MatchingOptionDto;
            mo.Should().NotBeNull();
            mo!.SourceType.Should().Be("VietnameseText");
            mo.TargetType.Should().Be("EnglishText");
            mo.VietnameseText.Should().Be("Con mèo");
            mo.EnglishText.Should().Be("Cat");
        }
        #endregion

        #region 5) Pronunciation => success
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_Pronunciation()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 3;
            var fakeLesson = new Lesson("Lesson Pronun", xpEarned: 10, order: 1);

            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            var qResult = Question.Create(
                instruction: "Say 'Hello'",
                vietnameseText: null,
                audio: null,
                englishText: "Hello",
                image: null,
                type: QuestionType.Pronunciation,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            var question = qResult.Value;

            var opt = Option.Create("Hãy nói Hello", null, null, "Hello").Value;
            var pronunRes = PronunciationQuestionOption.Create(question, opt, order: 1);
            var pronunOpt = pronunRes.Value;

            question.AddOption(pronunOpt);
            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Type.Should().Be("Pronunciation");
            dto.Options.Should().HaveCount(1);

            var pronDto = dto.Options[0] as PronunciationOptionDto;
            pronDto.Should().NotBeNull();
            pronDto!.EnglishText.Should().Be("Hello");
            pronDto.VietnameseText.Should().Be("Hãy nói Hello");
        }
        #endregion

        #region 6) BuildSentence => success
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_BuildSentence()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 4;
            var fakeLesson = new Lesson("Lesson BuildSent", xpEarned: 10, order: 1);

            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            var qResult = Question.Create(
                instruction: "Arrange the sentence",
                vietnameseText: null,
                audio: null,
                englishText: "Hello world",
                image: null,
                type: QuestionType.BuildSentence,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            var question = qResult.Value;

            var opt = Option.Create("Hello", null, null, null).Value;
            var bsRes = BuildSentenceQuestionOption.Create(question, opt, position: 1, order: 1);
            var bsOpt = bsRes.Value;

            question.AddOption(bsOpt);
            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Type.Should().Be("BuildSentence");
            dto.Options.Should().HaveCount(1);

            var buildDto = dto.Options[0] as BuildSentenceOptionDto;
            buildDto.Should().NotBeNull();
            // Because we used Option.Create("Hello",...) => that goes into "VietnameseText"
            // if your domain is consistent with the constructor. Adjust if needed.
            buildDto!.VietnameseText.Should().Be("Hello");
            buildDto.Order.Should().Be(1);
        }
        #endregion

        #region 7) Question with no Words => empty Words
        [Fact]
        public async Task Handle_Should_ReturnSuccess_WithEmptyWords_When_NoWordsInQuestion()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 5;
            var fakeLesson = new Lesson("Lesson no words", xpEarned: 5, order: 1);

            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            var qResult = Question.Create(
                instruction: "No words example",
                vietnameseText: "Không có words",
                audio: null,
                englishText: "NoWords",
                image: null,
                type: QuestionType.MultipleChoice,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            var question = qResult.Value;

            // No questionWord
            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Words.Should().BeEmpty();
            dto.EnglishText.Should().NotBeNull();
        }
        #endregion

        #region 8) Question with multiple words => success
        [Fact]
        public async Task Handle_Should_ReturnSuccess_WithWords_When_QuestionHasWords()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 6;
            var fakeLesson = new Lesson("Lesson has words", xpEarned: 0, order: 1);

            // Create configs
            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            var qResult = Question.Create(
                instruction: "Words example",
                vietnameseText: null,
                audio: null,
                englishText: "Check words",
                image: null,
                type: QuestionType.MultipleChoice,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            var question = qResult.Value;

            // Create Word domain objects
// 1) Tạo Media cho "hello.mp3"
var mediaResult1 = Media.CreateForTest(
    mimeType: MediaType.Audio,           // Loại file: Audio
    url: "http://media/hello.mp3",       // Đường dẫn
    fileSize: 1234                       // Kích thước
);

var audioMedia1 = mediaResult1.Value;

// 2) Tạo Word "Hello" kèm media audio
var w1 = Word.Create("Hello", audioMedia1).Value;

// Tương tự cho “World”
var mediaResult2 = Media.CreateForTest(
    mimeType: MediaType.Audio,
    url: "http://media/world.mp3",
    fileSize: 2345
);

var audioMedia2 = mediaResult2.Value;

var w2 = Word.Create("World", audioMedia2).Value;


            // Create QuestionWord
            var qw1 = QuestionWord.Create(w1, question, order: 1).Value;
            var qw2 = QuestionWord.Create(w2, question, order: 2).Value;

            question.AddWord(qw1);
            question.AddWord(qw2);

            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Words.Should().HaveCount(2);
            dto.Words[0].Word.Should().Be("Hello");
            dto.Words[1].Word.Should().Be("World");
        }
        #endregion

        #region 9) Unknown question type => default => empty Options
        [Fact]
        public async Task Handle_Should_ReturnSuccess_WithEmptyOptions_When_UnknownQuestionType()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var questionOrder = 99;
            var fakeLesson = new Lesson("Weird question type", xpEarned: 1, order: 1);

            // Create configs
            var questionConfig = Configuration.Create(false, false, false, false, false).Value;
            var optionConfig   = Configuration.Create(false, false, false, false, false).Value;

            // Force type to (QuestionType)999
            var qResult = Question.Create(
                instruction: "???",
                vietnameseText: null,
                audio: null,
                englishText: "???",
                image: null,
                type: (QuestionType)999,
                questionConfiguration: questionConfig,
                optionConfiguration: optionConfig,
                order: questionOrder
            );
            var question = qResult.Value;

            fakeLesson._questions.Add(question);

            _lessonRepositoryMock
                .Setup(r => r.GetLessonByIdAsync(lessonId))
                .ReturnsAsync(fakeLesson);

            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue("we handle unknown question type as success but empty Options");
            var dto = result.Value;
            dto.Type.Should().Be("999"); // because enum toString => "999"
            dto.Options.Should().BeEmpty();
        }
        #endregion

    }
}
