If you upgraded from a previous version of ASP.NET Web API, make sure that the
following line is present at the beginning of the Register method in App_Start\WebApiConfig.cs:
   config.MapHttpAttributeRoutes();

You may need to change the registration call in global.asax.cs from
   WebApiConfig.Register(GlobalConfiguration.Configuration);
to
   GlobalConfiguration.Configure(WebApiConfig.Register);

For more information on attribute routing in ASP.NET Web API,
see http://go.microsoft.com/fwlink/?LinkID=325423
