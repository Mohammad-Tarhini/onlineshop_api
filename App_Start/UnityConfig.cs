using onlineshopowner_api.App_Start.Setting;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Services;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Domain.Interfaces;
using onlineshopowner_api.Infrastructure.MappingDomainModel;
using onlineshopowner_api.Infrastructure.Models;
using onlineshopowner_api.Infrastructure.Repositories;
using onlineshopowner_api.Infrastructure.Token;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using System.Configuration;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.onlineshopowner_api.Infrastructure.ExternalServices;

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

            container.RegisterType<IjwtTokenGenerator, JwtTokenGenerator>();
            container.RegisterType<IUnityOfWork, UnitOfWork>();
            container.RegisterType<IUserContextService, UserContextServices>();

            container.RegisterType<IpersonRepository, PersonRepository>();
            container.RegisterType<IShopRepository, ShopRepository>();
            container.RegisterType<IcategoryRepository, CategoryRepository>();
           container.RegisterType<IOrderServices,OrderServices>();
            container.RegisterType<IAuthoServices, AuthoServices>();
            container.RegisterType<IProductServices, ProductServices>();
container.RegisterType<IAuthHelper, AuthHelper>();
            container.RegisterType<IImageService, ImageService>();
            container.RegisterType<IAddCategoryservices, AddCategoryservices>();
            container.RegisterType<IshopServices, ShopServices>();
            container.RegisterType<IRedisRepository, RedisRepository>();
            container.RegisterType<IImgur, Imgur>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IMapper<Domain.Entities.Client, Client>, ClientMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IMapper<Domain.Entities.Person, Person>, PersonMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IMapper<Domain.Entities.ShopOwner, ShopOwner>, ShopOwnerMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IMapper<Domain.Entities.shop, Shop>, ShopMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IMapper<Domain.Entities.Admin, admain>, AdminMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IRedisCacheService, RedisCacheService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGoogleMapService,GoogleMapService>();
            container.RegisterType<IDeliveryServices, DeliveryServices>();
            container.RegisterType<IDelivaryRepository, DelivaryRepository>();
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