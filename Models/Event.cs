using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class Event : IValidatableObject
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        public ApplicationUser Owner { get; set; }
        
        [Required(ErrorMessage = "Start Date/Time is required")]
        [Display(Name = "Start Date/Time:")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End Date/Time is required")]
        [Display(Name = "End Date/Time:")]
        public DateTime EndDateTime { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (EndDateTime < StartDateTime)
            {
                yield return new ValidationResult("End Date/Time must occur after Start Date/Time");
            }
        }
    }
}