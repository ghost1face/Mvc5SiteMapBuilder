# Mvc5SiteMapBuilder

Mvc5SiteMapBuilder is a tool that provides flexible menus, breadcrumb trails, similar to the ASP.NET SiteMapProvider model.
This code has been ported and adapted from project (https://github.com/maartenba/MvcSiteMapProvider) to meet specific needs of supporting DistributedCaching.

This code has various changes created to alter the original behavior and re-work the caching side of things to work in a web-farm environment and support the use of a distributed cache server (Redis, AppFabric, Memcached, etc).  It can be used for menus (permission/claims based), site maps, site map paths.  This project supports xml, json and code driven sitemaps.

# NuGet
    Install-Package Mvc5SiteMapBuilder

# License
