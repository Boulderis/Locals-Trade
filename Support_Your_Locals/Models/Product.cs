﻿namespace Support_Your_Locals.Models
{
    public class Product
    {
        //Binded with add advertisement.
        public long ProductID { get; set; }
        public string Name { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Unit { get; set; }
        public string Comment { get; set; }
        public long BusinessID { get; set; }
        public Business Business { get; set; }
    }
}
