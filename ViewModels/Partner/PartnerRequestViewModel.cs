using System.ComponentModel.DataAnnotations;
using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Partner
{
    public class PartnerRequestViewModel
    {
        [Required(ErrorMessage = "Partner type is required.")]
        public PartnerTypeEnum Type { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$",
            ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(150, ErrorMessage = "Email must be at most 150 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(
            @"^(?:\+355|355|0)(6[7-9]\d{7})$",
            ErrorMessage = "Phone number must be a valid Albanian mobile number.")]
        public string PhoneNumber { get; set; } = "";

        [StringLength(200, ErrorMessage = "Address must be at most 200 characters.")]
        public string? Address { get; set; }
    }
}
