﻿namespace JusTeeth.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Department
    {
        [Key]
        public int Id { get; set; }
    
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }

        public virtual Workplace Workplace { get; set; }

        public virtual ICollection<ApplicationUser> Employees { get; set; }
    }
}
