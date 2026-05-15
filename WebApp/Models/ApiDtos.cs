namespace WebApp.Models
{
    public class EventPerformerLinkRequest
    {
        public int? EventID { get; set; }
        public int? PerformerID { get; set; }
    }

    public class EventRegistrationRequest
    {
        public int? EventID { get; set; }
        public int? UserID { get; set; }
    }
}
