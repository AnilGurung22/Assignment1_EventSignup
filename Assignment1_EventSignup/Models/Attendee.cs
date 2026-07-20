using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Assignment1_EventSignup.Models
{
    public class Attendee
    {
        // Stored as a string per the assignment spec (e.g. GUID)
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        public int EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        [JsonIgnore]
        public Event? Event { get; set; }
    }
}
