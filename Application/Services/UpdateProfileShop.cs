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
    public class UpdateProfileShop:IUpdateProfileShop
    {
        private IImageService _imageservice;
        private readonly IUserContextService _usercontext;
        private readonly IUnityOfWork _Unityofwork;
        private readonly IImgur _imgur;
        private int _userId;


        public UpdateProfileShop(IImageService imageservice,IUserContextService usercontext,IUnityOfWork unityofwork,IImgur imgur)
        {
            _imageservice = imageservice;
            _usercontext = usercontext;
            _Unityofwork = unityofwork;
            _imgur = imgur;
           
        }
        public async Task<(bool IsSuccess, string message)> PutProfileForShop(UpdatProfileShopeDto dto)
        {
            try
            {
                _userId = _usercontext.GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }
            if (dto.logo_url == null && dto.File == null) return (false, "there are no image ");
            string thelogourl = null;
            //check  if this user is the shopowner for this shop 

            Domain.Entities.ShopOwner shopowner;
            try
            {
                var personResultCheckdb = await _Unityofwork.PersonRepository.GetPersonByPersonId(_userId);

                if (!personResultCheckdb.IsSuccess) return (false, personResultCheckdb.Error);
                if (!personResultCheckdb.IsFound) return (false, personResultCheckdb.Error);
                try
                {
                    var shopownerResultCheckdb = await _Unityofwork.PersonRepository.GetShopOwnerByPersonAsync(personResultCheckdb.Value);
                    if (!shopownerResultCheckdb.IsSuccess) return (false, shopownerResultCheckdb.Error);
                    if (!shopownerResultCheckdb.IsFound) return (false, shopownerResultCheckdb.Error);

                    shopowner = shopownerResultCheckdb.Value;

                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            catch (Exception ex) { return (false, ex.Message); }
            Domain.Entities.shop shopentity;
            try
            {
                var shop = await _Unityofwork.ShopRepository.GetShopByShopOwner(shopowner);

                if (!shop.IsSuccess) return (false, shop.Error);
                if (!shop.IsFound) return (false, shop.Error);
                shopentity = shop.Value;
                if (shop.Value.shopid != dto.shopid)
                {
                    return (false, "sorry you  are not the shopowner for this shop ");
                }
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }


            //add image to the cloud 
            string deletehash ;
            try
            {
                bool issuccessputimageincloud;
                string logourlorerror;
               


                if (dto.logo_url != null)
                {
                    (issuccessputimageincloud, logourlorerror, deletehash) = await _imageservice.ProcessImageAsync(100, 199, 100, imageUrl: dto.logo_url);
                }
                else if (dto.File != null)
                {
                    (issuccessputimageincloud, logourlorerror, deletehash) = await _imageservice.ProcessImageAsync(100, 199, 100, file: dto.File);
                }
                else
                {
                    issuccessputimageincloud = false;
                    logourlorerror = "no image is asigned";
                }

                if (!issuccessputimageincloud)
                {
                   return (false,logourlorerror);

                }
                else
                {
                    thelogourl = logourlorerror;
                    return (true, thelogourl);
                }
                
            }
            catch
            {
                return (false, "dengorous in entering image ");
            }

            if (shopentity.logoUrl != null)
            {
                var (issucces, message) = await _imgur.DeleteImageAsync(shopentity.deletehashingimage);
                if (!issucces) {
                    return (false, "the old  image is not deleted on cloud  ");
                }
            }
            //  add to database

            try
            {
                var updateLogourlondatabase = await _Unityofwork.ShopRepository.Updatethelogourl(thelogourl, deletehash,dto.shopid);
                if (updateLogourlondatabase == UpdateDataProcess.Success)
                {
                    return (true, "the new profile is put ");
                }
                else
                {
                    return (false, "error in updating database ");
                }


            }
            catch (Exception ex) 
            {
                return (false, ex.Message);
            }
           

        }
    }
}