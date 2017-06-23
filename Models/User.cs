using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace exam.Models {
    public class User : BaseEntity {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public List<Part> People { get; set; }

        public List<Activity> Activity { get; set; }
        public User() {
            People = new List<Part>();
            Activity = new List<Activity>();
        }
    }
}