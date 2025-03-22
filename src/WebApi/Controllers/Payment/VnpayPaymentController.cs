using System.Net.Sockets;
using Application.Common.Utls;
using Infrastructure.Services.Payment.Vnpay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace WebApi.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnpayPaymentController : ControllerBase
    {
        private readonly  IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        public VnpayPaymentController(IVnpay vnpay, IConfiguration configuration)
        {
            _vnpay = vnpay;
            _configuration = configuration;
            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
        }

        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
[HttpGet("CreatePaymentUrl")]
public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
{
    PrintUtils.PrintAsJson(moneyToPay);
    PrintUtils.PrintAsJson(description);

    string ipAddress;
    try
    {
        ipAddress = NetworkHelper.GetIpAddress(HttpContext);
    }
    catch (SocketException ex)
    {
        // Ghi log lỗi nếu cần
        Console.WriteLine( "Lỗi khi lấy IP từ NetworkHelper, sử dụng IP từ Connection trực tiếp.");
        ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }

    PrintUtils.PrintAsJson(ipAddress);

    var request = new PaymentRequest
    {
        PaymentId = DateTime.Now.Ticks,
        Money = moneyToPay,
        Description = description,
        IpAddress = ipAddress,
        BankCode = BankCode.ANY,
        CreatedDate = DateTime.Now,
        Currency = Currency.VND,
        Language = DisplayLanguage.Vietnamese
    };

    PrintUtils.PrintAsJson(request);
    var paymentUrl = _vnpay.GetPaymentUrl(request);
    return Created(paymentUrl, paymentUrl);
}


            /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
     [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            Console.WriteLine("IPN action");
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        Console.WriteLine("Thanh toán thành công");
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }

                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
           /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("Callback")]
        public IActionResult Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        Console.WriteLine("Thanh toán thành công");

                        return Ok(paymentResult);
                    }
                        Console.WriteLine("Thanh toán Thất bại");

                    return BadRequest(paymentResult);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
