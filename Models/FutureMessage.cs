using System.ComponentModel.DataAnnotations;

namespace LetterToSelf.Models
{
    public class FutureMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        public DateTime CurrentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Gönderim tarihi zorunludur.")]
        [DataType(DataType.Date, ErrorMessage = "Geçerli bir tarih giriniz.")]
        public DateTime SendDate { get; set; }

        public bool IsSent { get; set; }
    }
}