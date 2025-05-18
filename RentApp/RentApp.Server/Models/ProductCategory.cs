
using System.ComponentModel.DataAnnotations;
namespace RentApp.Server.Models
{
    public enum ProductCategory
    {
        [Display(Name = "Electronice")]
        Electronice,

        [Display(Name = "Mobilier")]
        Mobilier,

        [Display(Name = "Vehicule")]
        Vehicule,

        [Display(Name = "Unelte")]
        Unelte,

        [Display(Name = "Imbracaminte")]
        Imbracaminte,

        [Display(Name = "Carti")]
        Carti,

        [Display(Name = "Jucarii")]
        Jucarii,

        [Display(Name = "Sport")]
        SportSiTimpLiber,

        [Display(Name = "Audio-Video")]
        EchipamenteAudioVideo,

        [Display(Name = "Altele")]
        Altele
    }
}
