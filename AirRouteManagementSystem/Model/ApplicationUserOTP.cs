namespace AirRouteManagementSystem.Model
{
    public class ApplicationUserOTP
    {
        public int id {  get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string OtpCode { get; set; }
        public bool Isvalid { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ValidTo { get; set; }

    }
}
