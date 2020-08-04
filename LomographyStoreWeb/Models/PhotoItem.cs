using System;

namespace LomographyStoreWeb.Models
{
    public class PhotoItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Camera { get; set; }
        public string Film { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}