namespace BytexDigital.RGSM.Panel.Server.TransferObjects.Models
{
    public class UpdatePasswordModel
    {
        public string ApplicationUserId { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }
}
