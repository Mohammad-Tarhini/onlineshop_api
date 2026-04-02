using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.AuthoDto
{
    public class VerifyDeliveryRequestDto
    {
        public DeliveryProviderDto DeliveryProviderDto { get; set; }
        public VerifyOtpDto VerifyOtpDto { get; set; }
    }
}