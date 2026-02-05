namespace FaceLook.Services.Extensions
{
    public static class StringExtenstions
    {
        public static string ExtractControllerName(string controllerName)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(controllerName, nameof(controllerName));

            const string controllerSubstring = "Controller";
            if (!controllerName.Contains(controllerSubstring))
            {
                throw new Exception($"Argument {nameof(controllerName)}:{controllerName} is not valid controller name.");
            }
            return controllerName.Replace(controllerSubstring, string.Empty);
        }
    }
}
