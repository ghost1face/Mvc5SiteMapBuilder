﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimpleUseTestApplication.Startup))]
namespace SimpleUseTestApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            MvcSiteMapBuilder.SiteMapConfiguration.Init()
                .Build();
                
        }
    }
}
