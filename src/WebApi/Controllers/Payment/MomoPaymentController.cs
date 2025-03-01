using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Common.Utls;
using Domain.Entities.Payment;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Payment
{
    [ApiController]
    [Route("api/[controller]")]
    public class MomoPaymentController
    {
        private readonly IMomoService _momoPaymentService;
        public MomoPaymentController(IMomoService momoService)
        {
            _momoPaymentService = momoService;
        }
        [HttpPost]
    [Route("CreatePaymentUrl")]
    public async Task<Object> CreatePaymentUrl(OrderInfoModel model)
    {
        var responseModel = await _momoPaymentService.CreatePaymentAsync(model);
        PrintUtils.PrintAsJson(responseModel);
        
        return responseModel;
    }

    [HttpGet]
    [Route("PaymentExecute")]
    public async Task<IActionResult> PaymentExecuteAsync([FromQuery] IQueryCollection collection)
    {
        var responseModel = _momoPaymentService.PaymentExecuteAsync(collection);
        return new OkObjectResult(responseModel);

    }
}
}