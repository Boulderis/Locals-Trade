﻿using System.Collections.Generic;

namespace Support_Your_Locals.Models
{

    public class Business
    {
        public long BusinessID { get; set; }
        public long UserID { get; set; }
        public User User { get; set; }
        public string Description { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string PhoneNumber { get; set; }
        public string Header { get; set; }
        public List<TimeSheet> Workdays { get; set; } = new List<TimeSheet>();
        public IList<string> Pictures { get; set; } = new List<string>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
