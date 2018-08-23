using System.ComponentModel.DataAnnotations;

namespace RouteConfigurator.Model
{
    public class User
    {
        [Key]
        [Display(Name = "Email")]
        [MaxLength(50)]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(15)]
        [Required(ErrorMessage = "Name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(20)]
        [Required(ErrorMessage = "Name is required")]
        public string LastName { get; set; }

        [StringLength(64)]
        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(256)]
        [Required]
        public byte[] Salt { get; set; }

        //Manager or Supervisor
        [StringLength(15)]
        [Required(ErrorMessage = "Employee type is required")]
        public string EmployeeType { get; set; }
    }
}
