using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Services
{
    public class OpenNewShopServices:IOpenNewShopServices
    {
        private readonly IUserContextService _usercontext;
        private readonly IUnityOfWork _Unityofwork;
        
        private readonly IImageService _imageservice;
        private  int _userId;

        public OpenNewShopServices(IUserContextService usercontext, IUnityOfWork unityofwork,IImageService imageservice)
        {
            _usercontext = usercontext;
            _Unityofwork = unityofwork;
            _imageservice = imageservice;
            
        }
        public async Task<(bool IsSuccess, string message)> OpenShop(OpenNewShopDto dto)
        {
            try
            {
                _userId = _usercontext.GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }
            //check if userid is shopowner
            Domain.Entities.ShopOwner shopowner;
            try
            {
                var personResultCheckdb = await _Unityofwork.PersonRepository.GetPersonByPersonId(_userId);

                if (!personResultCheckdb.IsSuccess) return (false, personResultCheckdb.Error);
                if (!personResultCheckdb.IsFound) return (false, personResultCheckdb.Error);
                try
                {
                    var shopownerResultCheckdb =await _Unityofwork.PersonRepository.GetShopOwnerByPersonAsync(personResultCheckdb.Value);
                    if(!shopownerResultCheckdb.IsSuccess)return (false, shopownerResultCheckdb.Error);
                    if(!shopownerResultCheckdb.IsFound) return (false, shopownerResultCheckdb.Error);

                    shopowner = shopownerResultCheckdb.Value;

                }
                catch (Exception ex) { return (false,ex.Message); }
            }
            catch (Exception ex) { return (false, ex.Message); }

            //check if this shopowner have shop before and add directly
            var shop=await _Unityofwork.ShopRepository.GetShopByShopOwner(shopowner);
            if(!shop.IsSuccess)return (false, shop.Error);
            if(shop.IsFound) return (false, shop.Error);
            Clean.CleanStrings(dto);


           

            //add the shop
            try
            {
                var theshop = new Domain.Entities.shop(name: dto.Name, d: dto.Description, shopownerid: _userId);
                var addshop = await  _Unityofwork.ShopRepository.createShoponDatabase(theshop);
                if (addshop == UpdateDataProcess.Success)
                {
                    return (true, "congrats the new shop");
                }
                else { return (false, addshop.ToString()); }
                
            }
            catch (Exception ex) 
            {
                return (false, ex.Message);

            }





           

            

        }
        

        

    }
}