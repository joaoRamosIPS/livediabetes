using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LiveDiabetes.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(20)")]
        public string? FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(20)")]
        public string? LastName { get; set; }

        [PersonalData]
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(10)")]
        public string? Gender { get; set; }
    }
}
