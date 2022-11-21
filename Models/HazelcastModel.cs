using System.ComponentModel.DataAnnotations;

namespace HazelcastDemo.Models
{
    public class HazelcastModel
    {
        [Key]
        public string? HKey { get; set; }
        public string? HValue { get; set; }
    }
}
