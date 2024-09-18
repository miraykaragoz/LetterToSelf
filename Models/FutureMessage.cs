using System.ComponentModel.DataAnnotations;

namespace LetterToSelf.Models;

public class FutureMessage
{
    public int Id { get; set; }
    
    [Required]
    public string Message { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    public DateTime CurrentDate { get; set; } = DateTime.Now;
    
    public DateTime SendDate { get; set; }
    
    public bool IsSent { get; set; }
}