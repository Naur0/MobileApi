using System.ComponentModel.DataAnnotations;

namespace MobileApi.Models
{
    public class LostItemReport
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string ItemName { get; set; } = "";

        [Required, StringLength(600)]
        public string Description { get; set; } = "";

        [Required, StringLength(40)]
        public string Category { get; set; } = "";

        [Required]
        public DateTime LostDate { get; set; }

        [Required, StringLength(160)]
        public string LastSeenLocation { get; set; } = "";

        [Required, StringLength(60)]
        public string ContactName { get; set; } = "";

        [Required, StringLength(80)]
        public string ContactMethod { get; set; } = "";

        [Required, StringLength(20)]
        public string Status { get; set; } = "Open";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
