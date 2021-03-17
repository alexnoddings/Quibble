using System.ComponentModel.DataAnnotations;

namespace BlazorIdentityBase.Shared.Authentication
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
