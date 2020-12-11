﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MSupportYourLocals.Models;

namespace MSupportYourLocals.Services
{
    public interface IBusinessService
    {
        Task<PageBusiness> GetBusinesses(int page);
        Task<PageBusiness> GetFilteredBusinesses(string ownersSurname, string businessInfo, int searchIn, bool[] weekdaySelected, DateTime openFrom, DateTime openTo, int page);
        Task<ObservableCollection<Business>> GetUserBusinesses();
        Task DeleteBusiness(string password, long businessId);
        Task CreateBusiness(Business business);
        Task UpdateBusiness(string password, Business business);
    }
}
