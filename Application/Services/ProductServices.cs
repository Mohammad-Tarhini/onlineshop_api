using Antlr.Runtime.Tree;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.ProductDtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.Repositories;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace onlineshopowner_api.Application.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IUserContextService usercontext;
        private readonly IUnityOfWork unityofwork;
        private readonly IImageService imageservice;
        private readonly IRedisRepository redisRepository;
        private readonly Imgur imgur;

        public ProductServices(IUserContextService userContextService, IUnityOfWork unityOfWork, IRedisRepository redisRepository, IImageService imageService, Imgur imgur)
        {
            usercontext = userContextService;
            unityofwork = unityOfWork;
            this.redisRepository = redisRepository;
            imageservice = imageService;
            this.imgur = imgur;
        }


        public async Task AddProductByShopOwner(ProductRequestDto dto)
        {
            int userId = usercontext.GetUserId();
            string role = usercontext.GetUserRole();
            if (role.ToLower().Trim() != "shopowner")
                throw new UnauthorizedAccessException("is not shopowner");
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            var shopOwnerId = await unityofwork.PersonRepository.GetShopOwnerIdByPersonId(userId);
            if (shopOwnerId == null || shopOwnerId == 0)
                throw new Exception("the shopownerId is not exist ");
            var shopId = await unityofwork.ShopRepository.GetShopIDByShopownerId(shopOwnerId.Value);
            if (shopId == null || shopId == 0)
                throw new Exception("the shop is not exist ");
            string ImageNewUrl = null;
            string deleteUrl = null;
            if (dto.File != null || !string.IsNullOrEmpty(dto.ImgUrl))
            {
                (ImageNewUrl, deleteUrl) = await imageservice.ProcessImageAsync(100, 199, 100, imageUrl: dto.ImgUrl, file: dto.File);
            }

            var product = new Domain.Entities.Product
            {
                name = dto.Name,
                description = dto.Description,
                price = dto.Price,
                category_id= dto.CategoryId,
                shop_id = shopId.Value,
                quentity = dto.Quantity,
                status = dto.Status,
                imageurl = ImageNewUrl,

            };
            var result = await unityofwork.ProductRepository.addproduct(product);
            if (result == null)
            {
                throw new Exception("error in add product");
            }


        }
        public async Task UpdateProduct(ProductRequestDto updatedProductDto)
        {
            if (updatedProductDto == null) { throw new ArgumentNullException(nameof(updatedProductDto)); }
            int userId = usercontext.GetUserId();
            string role = usercontext.GetUserRole();
            if (role.ToLower().Trim() != "shopowner")
                throw new UnauthorizedAccessException("is not shopowner");
            var shopOwnerId = await unityofwork.PersonRepository.GetShopOwnerIdByPersonId(userId);
            if (shopOwnerId == null || shopOwnerId == 0)
                throw new Exception("the shopownerId is not exist ");
            var shopId = await unityofwork.ShopRepository.GetShopIDByShopownerId(shopOwnerId.Value);
            if (shopId == null || shopId == 0)
                throw new Exception("the shop is not exist ");
            var existingProductResult = await unityofwork.ProductRepository.GetProductById(updatedProductDto.Id);
            if (existingProductResult == null)
                throw new Exception("there isn no product ");
            if (updatedProductDto.Name != null)
            {
                existingProductResult.name = updatedProductDto.Name;
            }
            if (updatedProductDto.Quantity != null || updatedProductDto.Quantity == 0)
            {
                existingProductResult.quentity = updatedProductDto.Quantity;
            }
            if (updatedProductDto.CategoryId != null || updatedProductDto.CategoryId == 0)
            {
                existingProductResult.category_id = updatedProductDto.CategoryId;
            }
            if (updatedProductDto.Description != null)
            {
                existingProductResult.description = updatedProductDto.Description;
            }
            if (updatedProductDto.Price != null || updatedProductDto.Price == 0)
            {
                existingProductResult.price = updatedProductDto.Price;
            }
            if (updatedProductDto.Status != null)
            {
                existingProductResult.status = updatedProductDto.Status;
            }
            if (updatedProductDto.ImgUrl != null || updatedProductDto.File != null)
            {

                await imgur.DeleteImageAsync(existingProductResult.img_delete_code);

                string ImageNewUrl = null;
                string deleteUrl = null;

                (ImageNewUrl, deleteUrl) = await imageservice.ProcessImageAsync(100, 199, 100, imageUrl: updatedProductDto.ImgUrl, file: updatedProductDto.File);
                existingProductResult.imageurl = ImageNewUrl;
            }
            await unityofwork.ProductRepository.updateProduct(existingProductResult);




        }

        public async Task<(List<ProductReturnDto>, int limit, int page)> GetProducts(int shopid = 0, int limit = 30, int page = 1, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null)
        {
            (var products, var limited, var offset) = await unityofwork.ProductRepository.GetproductsToUser(shopid, limit, page, searchbyproductname, searchbycategory);

            if (products == null)
                throw new Exception("error in get products");
            var productdtos = new List<ProductReturnDto>();
            foreach (Product product in products)
            {
                var productreturndto = new ProductReturnDto
                {
                    Id = product.product_id,
                    Name = product.name,
                    Price = product.price,
                    Description = product.description,
                    ImgUrl = product.imageurl,
                    Status = product.status,
                    Category = product.category,
                };
                productdtos.Add(productreturndto);

            }
            return (productdtos, limited, offset);

        }
    }
}