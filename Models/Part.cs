using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace exam.Models {
    public class Part : BaseEntity {
        public int id { get; set; }
        public int userId { get; set; }
        public int activityId { get; set; }
        public User User { get; set; }


        public Activity Activity { get; set; }

    }
}