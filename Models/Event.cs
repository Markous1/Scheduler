using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class Event
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
    }
}
