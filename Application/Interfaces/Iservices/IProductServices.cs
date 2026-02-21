using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IProductServices
    {

        Task AddProductByShopOwner(ProductRequestDto dto);
        Task UpdateProduct(ProductRequestDto updatedProductDto);
        Task<(List<ProductReturnDto>, int limit, int page)> GetProducts(int shopid = 0, int limit = 30, int page = 1, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null);
        //Task<(bool issuccess, string message)> addproduct(ProductDto dto);
        //Task<(bool IsSuccess, string message)> AddImageProduct(AddProductImageDto dto,bool isprofile);
        //Task<(bool issucess, List<ProductDto> productDtos, string message)> GetProducts(int shopid = 0, int limit = 30, int page = 1, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null);
    }
}
