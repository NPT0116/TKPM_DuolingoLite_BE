using System;
using Gemini.NET;
using Models.Enums;
using Models.Request;

namespace Infrastructure.Config;

    public class GeminiConfig
    {
        // API key dùng để xác thực với Gemini API.
        public string ApiKey { get; private set; }
        
        // Mức độ ngẫu nhiên cho kết quả sinh ra, mặc định là 0.7
        public float Temperature { get; set; } = 0.7F;
        
        // Kiểu dữ liệu trả về từ API, mặc định là PlainText.
        public ResponseMimeType ResponseMimeType { get; set; } = ResponseMimeType.PlainText;

        // (Có thể bổ sung các thuộc tính khác nếu cần)

        // Constructor bắt buộc phải cung cấp API key.
        public GeminiConfig(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key must not be null or empty", nameof(apiKey));
            
            ApiKey = apiKey;
        }

        /// <summary>
        /// Tạo một instance của Generator từ Gemini.NET sử dụng API key đã cấu hình.
        /// </summary>
        public Generator CreateGenerator()
        {
            return new Generator(ApiKey);
        }

        /// <summary>
        /// Xây dựng một ApiRequest từ prompt và (tuỳ chọn) system instruction,
        /// sử dụng cấu hình mặc định đã cài đặt như Temperature và ResponseMimeType.
        /// </summary>
        /// <param name="prompt">Nội dung prompt để sinh kết quả</param>
        /// <param name="systemInstruction">(Tuỳ chọn) Hướng dẫn hệ thống cho Gemini</param>
        /// <returns>ApiRequest đã được xây dựng</returns>
        public ApiRequest BuildApiRequest(string prompt, string systemInstruction = null)
        {
            Console.WriteLine("Prompt: " + prompt);
            var apiRequest = new ApiRequestBuilder()
                .WithPrompt( prompt)
                .WithDefaultGenerationConfig(temperature: 0.7F)
                .WithSystemInstruction(systemInstruction)
                .Build();
            
            return apiRequest;
        }
    }