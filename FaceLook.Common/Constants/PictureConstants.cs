namespace FaceLook.Common.Constants
{
    public static class PictureConstants
    {
        public static readonly IReadOnlyCollection<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];

        public const string PictureMessageKey = "PictureMessage";
        public const string PictureErrorKey = "PictureError";
        public const string PictureNameKey = "PictureName";
        public const string PictureSasUriKey = "PictureSasUri";
        public const string PictureSasExpiresOnKey = "PictureSasExpiresOn";
    }
}
