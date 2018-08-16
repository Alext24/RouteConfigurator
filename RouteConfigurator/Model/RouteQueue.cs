using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model
{
    [Table("RouteQueue")]
    public class RouteQueue
    {
        [Key]
        [Display(Name = "Route Queue ID")]
        public int RouteQueueID { get; set; }

        public int Route { get; set; }

        [Display(Name = "Model Number")]
        public string ModelNumber { get; set; }

        [Display(Name = "Line")]
        public string Line { get; set; }

        public decimal TotalTime { get; set; }
        
        //True if the route has been submitted to SAP, false if waiting to be submitted
        public Boolean IsApproved { get; set; }

        //Day the route has been queued to the database
        [Display(Name = "Added Date")]
        [Column(TypeName = "date")]
        public DateTime AddedDate { get; set; }

        //Day the route has been sent to SAP
        [Display(Name = "Submitted Date")]
        [Column(TypeName = "date")]
        public DateTime SubmittedDate { get; set; }
    }
}
