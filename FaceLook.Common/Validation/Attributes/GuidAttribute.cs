using System.ComponentModel.DataAnnotations;

namespace FaceLook.Common.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
       AllowMultiple = false)]
    public class GuidStringAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            if (!Guid.TryParse(value.ToString(), out Guid _)) return false;

            return true;
        }
    }
}
