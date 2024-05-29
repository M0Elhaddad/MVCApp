using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class SignUpViewModel
	{
        [Required(ErrorMessage ="User Name is Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="First Name is Required")]
        public string FName { get; set; }
		[Required(ErrorMessage = "Last Name is Required")]
		public string LName { get; set; }
        [Required(ErrorMessage ="Email Is Required")]
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password Is Required")]
        [MinLength(5,ErrorMessage ="Minimum Password Length is 5")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
		[Required(ErrorMessage = "Password Is Required")]
        [Compare(nameof(Password),ErrorMessage ="Confirm Password does not match Password")]
		[DataType(DataType.Password)]
		public string ConfirmPasseword { get; set; }
        [Required(ErrorMessage ="This Check is Required")]
        public bool IsAgree { get; set; }
    }
}
