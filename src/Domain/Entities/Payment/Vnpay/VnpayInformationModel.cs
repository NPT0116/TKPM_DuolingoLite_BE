using System;

namespace Domain.Entities.Payment.Vnpay;

public class VnpayInformationModel
{
    public string OrderType { get; set; }
    public double Amount { get; set; }
    public string OrderDescription { get; set; }
    public string Name { get; set; }

}
