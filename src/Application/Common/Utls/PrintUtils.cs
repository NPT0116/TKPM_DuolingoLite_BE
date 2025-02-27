using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Utls;

 public static class PrintUtils
    {
        public static void PrintAsJson(object obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,  
                        // In ra format đẹp
                ReferenceHandler = ReferenceHandler.IgnoreCycles // Tránh loop nếu có quan hệ vòng
            };

            string jsonString = JsonSerializer.Serialize(obj, options);
            Console.WriteLine(jsonString);
        }

        // Hoặc bạn có thể viết thêm phiên bản generic (nếu muốn):
        public static void PrintAsJson<T>(T obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string jsonString = JsonSerializer.Serialize(obj, options);
            Console.WriteLine(jsonString);
        }
    }