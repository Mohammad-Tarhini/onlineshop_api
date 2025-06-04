using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.Models;
using onlineshopowner_api.Application.Interfaces;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Infrastructure.Repositories;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Services;
using onlineshopowner_api.Infrastructure.Token;
using onlineshopowner_api.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unity.Lifetime;
using Unity;
using AutoMapper;
using System.Runtime.InteropServices;
using Unity.WebApi;
using onlineshopowner_api.Domain.Interfaces;
using onlineshopowner_api.Infrastructure.MappingDomainModel;
using System.Reflection;

namespace onlineshopowner_api.App_Start
{
    public class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

           

            // Register your DbContext, Repositories, UoW, Services
            container.RegisterType<online_shopEntities>(new HierarchicalLifetimeManager());

            // Example:

            container.RegisterType<IjwtTokenGenerator, JwtTokenGenerator>();
            container.RegisterType<IUnityOfWork, UnitOfWork>();

            container.RegisterType<Iregistrationhelper, Registerationhelper>();
            container.RegisterType<IpersonRepository, PersonRepository>();
            container.RegisterType<IRegisterationServices, RegisterationServices>(new HierarchicalLifetimeManager());


            container.RegisterType<IMapper<Domain.Entities.Client, Client>, ClientMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IMapper<Domain.Entities.Person, Person>, PersonMapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IMapper<Domain.Entities.ShopOwner, ShopOwner>, ShopOwnerMapper>(new HierarchicalLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
