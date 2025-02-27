using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Features.Learning.Question.Queries.GetAQuestionFromLessionId;

namespace WebApi.Utils;

public class OptionDtoPolymorphicConverter : JsonConverter<OptionDto>
{
    public override OptionDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Nếu bạn cần deserialize, bạn phải parse "discriminator" để biết class con.
        throw new NotSupportedException("Deserialization not supported here.");
    }

    public override void Write(Utf8JsonWriter writer, OptionDto value, JsonSerializerOptions options)
    {
        // Serialize theo đúng lớp con
        var actualType = value.GetType();
        JsonSerializer.Serialize(writer, value, actualType, options);
    }
}