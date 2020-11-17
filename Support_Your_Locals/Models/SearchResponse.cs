﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Support_Your_Locals.Models
{
    public class SearchResponse
    {
        [FromQuery(Name="os")]
        public string OwnersSurname { get; set; }
        [FromQuery(Name = "bi")]
        public string BusinessInfo { get; set; }
        [FromQuery(Name = "si")]
        public int SearchIn { get; set; }
        [FromQuery(Name = "w")]
        public string WeekSelected { get; set; } = "1111111";
        public bool[] WeekdaySelected { get; set; } = new bool[7];
      
        public string ToQuery()
        {
            System.Diagnostics.Debug.WriteLine(DefaultSearchFields());
            if (DefaultSearchFields()) return "";
            return $"/?os={OwnersSurname}&bi={BusinessInfo}&si={SearchIn}&w={WeekSelected}";
        }

        private bool DefaultSearchFields()
        {
            return string.IsNullOrEmpty(OwnersSurname) && string.IsNullOrEmpty(BusinessInfo) && SearchIn == 0 && WeekSelected == "1111111";
        }

        public void SetWeekdaySelected()
        {
            if (WeekSelected.Length != 7) return;
            for (int i = 0; i < 7; i++)
            {
                if (WeekSelected[i] == '1') WeekdaySelected[i] = true;
                else WeekdaySelected[i] = false;
            }
        }

        public IEnumerable<Business> FilterBusinesses(IEnumerable<Business> businesses)
        {
            return businesses.Where(b => BusinessConditionsMet(b) && UserConditionsMet(b.User) && ChosenWeekday(b.Workdays));
        }

        private bool UserConditionsMet(User user)
        {
            if (!string.IsNullOrEmpty(OwnersSurname)) return ChosenOwnersSurname(user);
            return true;
        }

        private bool BusinessConditionsMet(Business business)
        {
            if (!String.IsNullOrEmpty(BusinessInfo))
            {
                if (SearchIn == 0)
                {
                    if (!ChosenHeader(business)) return false;
                }
                else if (SearchIn == 1)
                {
                    if (!ChosenDescription(business)) return false;
                }
                else if (!ChosenDescription(business) || ChosenHeader(business)) return false;
            }
            return true;
        }


        private bool ChosenHeader(Business business)
        {
            return business.Header.ToLower().Contains(BusinessInfo.ToLower());
        }

        private bool ChosenDescription(Business business)
        {
            return business.Description.ToLower().Contains(BusinessInfo.ToLower()); // OK, BusinessInfo is Description if search in description is ticked.
        }

        //TODO: Search by working hours.

        private bool ChosenWeekday(IEnumerable<TimeSheet> timeSheets)
        {
            return timeSheets.Count(t => WeekdaySelected[t.Weekday - 1]) > 0;
        }

        private bool ChosenOwnersSurname(User user)
        {
            return user.Surname.ToLower().Contains(OwnersSurname.ToLower());
        }

    }
}
