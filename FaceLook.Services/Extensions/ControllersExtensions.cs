using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Services.Extensions
{
    public static class ControllersExtensions
    {
        public static string ExtractControllerName<T>(this T controller)
            where T : Controller
        {
            Type controllerType = controller.GetType();

            return controllerType.Name.Replace("Controller", string.Empty);
        }

        public static string GetControllerName<T>()
            where T : Controller
        {
            Type ttype = typeof(T);

            return ttype.Name.Replace("Controller", string.Empty);
        }
    }
}
