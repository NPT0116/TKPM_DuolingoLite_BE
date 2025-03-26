using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.Hubs;
using Domain.Entities;
using Application.Interfaces;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Worker
{
    public class RemindUserReviewLesson : IJob
    {
        private readonly ISpacedRepetitionRepository _repository;
        private readonly IDistributedCache _cache;
        private readonly INotificationHub _hubContext;
        private readonly ILogger<RemindUserReviewLesson> _logger;

        public RemindUserReviewLesson(
            ISpacedRepetitionRepository repository,
            IDistributedCache cache,
            INotificationHub hubContext,
            ILogger<RemindUserReviewLesson> logger)
        {
            _repository = repository;
            _cache = cache;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            // Lấy danh sách userId đang online từ Hub
            var onlineUserIds = NotificationHub.ConnectedUsers.Keys.ToList();
            _logger.LogInformation($"Found {onlineUserIds.Count} online users");
            foreach (var userId in onlineUserIds)
            {
                // Truy vấn các bài ôn tập đến hạn cho user đó (ví dụ limit 10, không dùng cursor)
                var dueReviews = await _repository.GetDueReviewsAsync(Guid.Parse(userId), 10, null);
                if (dueReviews.Any())
                {
                    // Tạo key cache để kiểm tra thông báo đã được gửi chưa
                    var cacheKey = $"remindReviewLesson:{userId}";
                    var sentFlag = await _cache.GetStringAsync(cacheKey);
                    if (string.IsNullOrEmpty(sentFlag))
                    {
                        // Tổng hợp thông báo
                        var message = $"Bạn có {dueReviews.Count} bài ôn tập cần thực hiện!";
                        var notif  = Notification.Create(userId, NotificationType.ReviewLessonNoti, "Nhắc nhở ôn tập", message);

                        await _hubContext.ReviewNotification(userId, notif);
                        // Gửi thông báo qua SignalR tới group userId


                        // Lưu trạng thái thông báo với TTL là 1 giờ (có thể thay đổi thành 2 giờ nếu cần)
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                        await _cache.SetStringAsync(cacheKey, "sent", options);
                    }
                }
            }
        }
    }
}
