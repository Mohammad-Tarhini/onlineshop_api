using onlineshopowner_api.App_Start.Setting;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Services;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Interfaces;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.comunicate;
using onlineshopowner_api.Infrastructure.ExternalServices.googlemap;
using onlineshopowner_api.Infrastructure.ExternalServices.onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.Payment;
using onlineshopowner_api.Infrastructure.Models;
using onlineshopowner_api.Infrastructure.Repositories;
using onlineshopowner_api.Application.Services.AuthoServices;
using onlineshopowner_api.Infrastructure.Token;
using System.Configuration;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace onlineshopowner_api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.AddExtension(new Diagnostic());
            // Register your DbContext, Repositories, UoW, Services
            container.RegisterType<online_shopEntities1>(new HierarchicalLifetimeManager());

            // Example:

          //  container.RegisterType<IjwtTokenGenerator, VerificationTokenService>();
            container.RegisterType<IUnityOfWork, UnitOfWork>();
          


            //++++++++++Services++++++++++
            container.RegisterType<ILoginService, LoginServices>();
            container.RegisterType<IRegisterationServices, RegisterationService>();
            container.RegisterType<IProductServices, ProductServices>();
            container.RegisterType<IshopServices, ShopServices>();
            container.RegisterType<IAddCategoryservices, AddCategoryservices>();
            container.RegisterType<IDeliveryServices, DeliveryServices>();
            container.RegisterType<IOrderServices, PayAndRegisterOrder>();
            container.RegisterType<IAddCategoryservices, AddCategoryservices>();

            //++++++++++help services ++++++++++
            container.RegisterType<IImageService, ImageService>();
            container.RegisterType<IUserContextService, UserContextServices>();



            //++++++++++Repository++++++++++
            container.RegisterType<IpersonRepository, PersonRepository>();
            container.RegisterType<IShopRepository, ShopRepository>();
            container.RegisterType<IcategoryRepository, CategoryRepository>();
           container.RegisterType<IOrderServices,PayAndRegisterOrder>();
            container.RegisterType<IPaymentRepository, PaymentRepository>();
            container.RegisterType<IPaymentAndOrderRepository, PaymentAndOrderRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IDelivaryRepository, DelivaryRepository>();
            container.RegisterType<IRedisRepository, RedisRepository>();

            container.RegisterType<IDBTransaction, EfTransaction>();
            container.RegisterType<IUnityOfWork, UnitOfWork>();
            //++++++++++ExternalServices++++++++++
            container.RegisterType<IImgur, Imgur>();
            container.RegisterType<ITwilioMessageService, TwilioMessageService>();
            container.RegisterType<IFakeGatewayService, FakeGateWayService>();
            container.RegisterType<IGoogleMapService, GoogleMapService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IFakeGatewayService, FakeGateWayService>();

            container.RegisterType<IRedisCacheService, RedisCacheService>(new ContainerControlledLifetimeManager());


            //++++++++++++Token+++++++++++++++
            container.RegisterType<IjwtTokenGenerator, JwtTokenGenerator>();
            //container.RegisterType<I, JwtAuthorizeAttribute>();

            //            container.RegisterType<IAuthoServices, AuthoServices>();
            //            container.RegisterType<IProductServices, ProductServices>();
            //container.RegisterType<IAuthHelper, AuthHelper>();















            var imgurSettings = new ImgurSettings
            {
                ClientId = "6df469619d45ea8"
            };
            container.RegisterInstance(imgurSettings);
            var jwtSettings = new JwtSettings
            {
                SecretKey = ConfigurationManager.AppSettings["JwtSecretKey"],
                Issuer = ConfigurationManager.AppSettings["JwtIssuer"],
                Audience = ConfigurationManager.AppSettings["JwtAudience"]
            };
            container.RegisterInstance(jwtSettings);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}