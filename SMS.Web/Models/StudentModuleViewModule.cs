using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Web.Models
{
    public class StudentModuleViewModel
    {
        // selectlist of modules (id, title)       
        public SelectList Modules { set; get; }

        public int StudentId { get; set; }
       
        [Required]
        [Display(Name = "Select Module")]
        public int ModuleId { get; set; }
        
        [Range(0,100)]
        public int Mark { get; set; }
    }

}
