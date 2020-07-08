﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerRest.Models
{
    public class Job
    {
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Number users use to correlate a job when it is in the quote stage
        /// </summary>
        [Required]
        [MaxLength(12)]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Quote number must be numeric")]
        public string QuoteNumber { get; set; }

        /// <summary>
        /// Number users use to correlate a job when it is sold
        /// </summary>
        [MaxLength(12)]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Job number must be numeric")]
        public string JobNumber { get; set; }

        /// <summary>
        /// Indicates this job is sold or not
        /// </summary>
        [Required]
        public bool IsAJob { get; set; }
    }
}
