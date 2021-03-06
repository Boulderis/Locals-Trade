﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Support_Your_Locals.Models.ViewModels
{
    public class BusinessRegisterModel
    {

        [Required(ErrorMessage = "Please add your business description")]
        public string Description { get; set; }
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Bad phone number")]
        [Required(ErrorMessage = "Please enter your business phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter your business header")]
        public string Header { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public IFormFile Picture { get; set; }
        public TimeSheet[] Workdays { get; set; } = new TimeSheet[7];
        public List<ProductRegisterModel> Products { get; set; } = new List<ProductRegisterModel>();

        public IEnumerable<Product> ToProducts()
        {
            foreach(var p in Products)
            {
                if (p == null) continue;
                byte[] PictureData = null;
                if (p.Picture != null)
                {
                    MemoryStream imageMemoryStream = new MemoryStream();
                    p.Picture.CopyTo(imageMemoryStream);
                    PictureData = imageMemoryStream.ToArray();
                }
                yield return new Product { Name = p.Name, PricePerUnit = p.PricePerUnit, Unit = p.Unit, Comment = p.Comment, PictureData = PictureData };
            }
        }

        public void SetModelForUpdate(Business business)
        {
            Description = business.Description;
            PhoneNumber = business.PhoneNumber;
            Header = business.Header;
            Longitude = business.Longitude;
            Latitude = business.Latitude;
            Workdays = new TimeSheet[7];
            for(int i = 0; i < 7; i++)
            {
                int index = business.Workdays.FindIndex(ind => ind.Weekday == (i+1));
                if (index > -1)
                {
                    Workdays[i] = business.Workdays[index];
                }
            }
        }

    }
}
