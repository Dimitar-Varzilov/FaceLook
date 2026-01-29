namespace FaceLook.ViewModels
{
    public class BaseModifiedViewModel : BaseViewModel
    {
        public required Guid ModifiedBy { get; set; }
    }
}
