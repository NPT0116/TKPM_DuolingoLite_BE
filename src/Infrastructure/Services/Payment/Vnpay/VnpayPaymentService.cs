using System;
using Microsoft.Extensions.Configuration;
using VNPAY.NET;

namespace Infrastructure.Services.Payment.Vnpay;

public class VnpayPaymentService
{
    private readonly IVnpay _vnpay;
    private readonly IConfiguration _configuration;
    public VnpayPaymentService(IVnpay vnpayUrl, IConfiguration configuration)
    {
        _vnpay = vnpayUrl;
        _configuration = configuration;
        _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
    }   

}
