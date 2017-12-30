using System.ComponentModel.DataAnnotations;

namespace Authorization.Models.Register
{
    public class RegisterESPModel
    {
        [Required]
        [Display(Name = "ESP Unique Identifier")]
        public string ESPIdentifier { get; set; }

        public override string ToString()
        {
            return $"{nameof(ESPIdentifier)}: {ESPIdentifier}";
        }
    }
}