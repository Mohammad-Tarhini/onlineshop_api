using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserContextService _usercontext;
        public OrderServices(IProductRepository productRepository, IUserContextService userContext)
        {
            _productRepository = productRepository;
            _usercontext = userContext;
        }

        public async Task<(bool issucess, List<CartItemCheckResponseDto> cartItemsResponse, decimal Totalprice, string message)> CheckCartItemAvailability(CartAvailabilityRequestDto dto)
        {
            int shopid=dto.ShopId;
            var cartItemRequestDtos=dto.CartItemDtos;
            int _userId;
            string _role;
            int shopownerid;
            decimal totalprice = 0;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, null, 0, ex.Message);
            }
            try
            {
                _role = _role.ToLower();
                
                //check if shopowner
                if (_role != "client") return (false, null, 0, "user is not client  ");

                var result = new List<CartItemCheckResponseDto>();
                foreach (var item in cartItemRequestDtos)
                {
                    var resultproductDB = await _productRepository.GetProductById(item.ProductId);
                    if (resultproductDB == null)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName= item.ProductName,
                            IsAvailable = false,
                            Reason = "Product is not found "
                        });
                        continue;
                    }
                    if (resultproductDB.Value.shop_id != shopid)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName= item.ProductName,
                            IsAvailable = false,
                            Reason = "the product not in shhopid"
                        });
                        continue;
                    }
                    if (!resultproductDB.IsSuccess)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName=item.ProductName,   
                            IsAvailable = false,

                            Reason = "error in database "

                        });
                        continue;
                    }
                    if (!resultproductDB.IsFound)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName= item.ProductName,
                            IsAvailable = false,
                            Reason = "product not found"
                        });
                        continue;
                    }
                    if (!string.Equals(resultproductDB.Value.status?.Trim(), "available", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName= item.ProductName,
                            IsAvailable = false,
                            CurrentPrice=resultproductDB.Value.price,
                            Reason = "is not available"
                        });
                        continue;
                    }
                    int dif = resultproductDB.Value.quentity - item.Quantity;
                    if (dif < 0)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,
                            Reason = "the available only " + dif
                        });
                        continue;
                    }
                    result.Add(new CartItemCheckResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName = resultproductDB.Value.name,
                        IsAvailable = true,
                        AvailableQuantity = resultproductDB.Value.quentity,
                        CurrentPrice = resultproductDB.Value.price,

                    });
                    totalprice += resultproductDB.Value.price;



                }
                return (true, result, totalprice, "sucess");

            }
            catch (Exception ex) 
            {
                return (false, null, 0, ex.Message);
            }

        }
    }
}