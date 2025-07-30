using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IOrderServices
    {
        Task<(bool issucess, List<CartItemCheckResponseDto> cartItemsResponse, decimal Totalprice, string message)> CheckCartItemAvailability(CartAvailabilityRequestDto dto);
    }
}
