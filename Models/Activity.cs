using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace exam.Models {
    public class Activity : BaseEntity {
        public int id { get; set; }

        [Required(ErrorMessage="Activity needs a title")]
        [MinLength(2)]
        public string title { get; set; }

        [Required(ErrorMessage="Activity needs a time")]
        // [MinLength(2)]
        public string time { get; set; }

        [Required]
        // [MinLength(2, ErrorMessage="Date can't be left blank")]
        [DataType(DataType.Date, ErrorMessage = "Date can't be left blank")]
        // [RegularExpression(@"^(3[01]|[12][0-9]|0[1-9])[-/](1[0-2]|0[1-9])[-/][0-9]{4}$", ErrorMessage = "Date can't be left blank")]
        public DateTime date { get; set; }

        [Required(ErrorMessage="Activity needs a duration")]
        // [MinLength(2)]
        public string duration { get; set; }

        [Required(ErrorMessage="Activity needs a duration type")]
        // [MinLength(2)]
        public string type { get; set; }

        [Required(ErrorMessage="Activity needs a description")]
        [MinLength(10)]
        public string desc { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int userId { get; set; }

        public User User { get; set; }

        public List<Part> People { get; set; }
        public Activity() {
            People = new List<Part>();
        }
    }
}