using System;
using Domain.Entities.Payment;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interface;

public interface IMomoService
{
    public Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
    public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);

    
}
