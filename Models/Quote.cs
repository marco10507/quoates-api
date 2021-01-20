using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QuotesAPI.Models
{
    public class Quote{
        public Quote(){
            
        }

        public int Id { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [StringLength(20)]
        public string Type { get; set; }
        public DateTime PublicationDate { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
    }
}
