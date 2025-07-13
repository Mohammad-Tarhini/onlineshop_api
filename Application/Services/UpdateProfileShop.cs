using onlineshopowner_api.Application.Interfaces.Ivalidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Application.Dtos;
using System.Diagnostics;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;

namespace onlineshopowner_api.Application.Services
{
    public class UpdateProfileShop : IUpdateProfileShop
    {
        private IImageService _imageservice;
        private readonly IUserContextService _usercontext;
        private readonly IUnityOfWork _Unityofwork;
        private readonly IImgur _imgur;
        private int _userId;


        public UpdateProfileShop(IImageService imageservice, IUserContextService usercontext, IUnityOfWork unityofwork, IImgur imgur)
        {
            _imageservice = imageservice;
            _usercontext = usercontext;
            _Unityofwork = unityofwork;
            _imgur = imgur;

        }
        public async Task<(bool IsSuccess, string message)> PutProfileForShop(UpdatProfileShopeDto dto)
        {
            // Get user ID from token
            try
            {
                _userId = _usercontext.GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }

            // Check if image is provided
            if (dto.logo_url == null && dto.File == null)
                return (false, "No image provided.");

            // Check if this user is a valid shop owner
            var personResult = await _Unityofwork.PersonRepository.GetPersonByPersonId(_userId);
            if (!personResult.IsSuccess || !personResult.IsFound)
                return (false, personResult.Error);

            var shopOwnerResult = await _Unityofwork.PersonRepository.GetShopOwnerByPersonAsync(personResult.Value);
            if (!shopOwnerResult.IsSuccess || !shopOwnerResult.IsFound)
                return (false, shopOwnerResult.Error);

            var shopOwner = shopOwnerResult.Value;

            // Check if the shop belongs to the shop owner
            var shopResult = await _Unityofwork.ShopRepository.GetShopByShopOwner(shopOwner);
            if (!shopResult.IsSuccess || !shopResult.IsFound)
                return (false, shopResult.Error);

            var shopEntity = shopResult.Value;

            if (shopEntity.shopid != dto.shopid)
                return (false, "You are not the owner of this shop.");

            // Upload new image to cloud
            string logoUrl = null;
            string deleteHash = null;

            try
            {
                bool uploadSuccess;
                string cloudResponse;

                if (dto.logo_url != null)
                {
                    (uploadSuccess, cloudResponse, deleteHash) = await _imageservice.ProcessImageAsync(100, 199, 100, imageUrl: dto.logo_url);
                }
                else
                {
                    (uploadSuccess, cloudResponse, deleteHash) = await _imageservice.ProcessImageAsync(10000, 19900, 109990, file: dto.File);
                }

                if (!uploadSuccess)
                    return (false, cloudResponse);

                logoUrl = cloudResponse;
            }
            catch
            {
                return (false, "Failed to process image.");
            }

            // Delete old image if exists
            if (!string.IsNullOrWhiteSpace(shopEntity.logoUrl))
            {
                var (deleteSuccess, message) = await _imgur.DeleteImageAsync(shopEntity.deletehashingimage);
                if (!deleteSuccess)
                    return (false, "Old image deletion from cloud failed.");
            }

            // Update shop logo URL in DB
            try
            {
                var updateResult = await _Unityofwork.ShopRepository.Updatethelogourl(logoUrl, deleteHash, dto.shopid);
                if (updateResult == UpdateDataProcess.Success)
                    return (true, "Shop profile image updated successfully.");
                else
                    return (false, "Database update failed.");
            }
            catch (Exception ex)
            {
                return (false, $"Database update error: {ex.Message}");
            }
        }
    }
}