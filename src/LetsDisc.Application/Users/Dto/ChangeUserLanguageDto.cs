using System.ComponentModel.DataAnnotations;

namespace LetsDisc.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}