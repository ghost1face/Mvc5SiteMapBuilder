using System;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace MvcSiteMapBuilder.Web.Mvc
{
    public static class ControllerDescriptorFactory
    {
        public static ControllerDescriptor Create(Type controllerType)
        {
            ControllerDescriptor controllerDescriptor = null;
            if (typeof(IController).IsAssignableFrom(controllerType))
            {
                controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            }
            else if (typeof(IAsyncController).IsAssignableFrom(controllerType))
            {
                controllerDescriptor = new ReflectedAsyncControllerDescriptor(controllerType);
            }
            return controllerDescriptor;
        }
    }
}
