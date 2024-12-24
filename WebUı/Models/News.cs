namespace WebUı.Models
{
    public enum NewsStatus
    {
        Pending,    // Onay Bekliyor
        Approved,   // Onaylandı
        Rejected    // Reddedildi
    }

    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public int ViewCount { get; set; } = 0;
        public NewsStatus Status { get; set; } = NewsStatus.Pending;
        public string? RejectionReason { get; set; }  // Red edilme sebebi
        public DateTime? ApprovalDate { get; set; }   // Onaylanma tarihi
        public string? ApprovedByUserId { get; set; } // Onaylayan kullanıcı

        // Foreign Keys
        public int CategoryId { get; set; }
        public string? UserId { get; set; }

        // Navigation Properties
        public Category Category { get; set; }
        public AppUser? User { get; set; }
        public AppUser? ApprovedByUser { get; set; }
    }
}
