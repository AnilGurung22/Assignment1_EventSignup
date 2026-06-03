namespace Assignment1_EventSignup.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public List<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}