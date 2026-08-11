using System.ComponentModel.DataAnnotations;

namespace Assignment1_EventSignup.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        // URL of the banner image uploaded to Azure Blob Storage
        public string? BannerUrl { get; set; }

        // Id of the Organizer (IdentityUser) who owns this event
        public string? OrganizerUserId { get; set; }

        public List<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}