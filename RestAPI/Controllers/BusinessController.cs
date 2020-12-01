﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using RestAPI.Models.BindingTargets;
using RestAPI.Models.Repositories;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private IServiceRepository repository;

        public BusinessController(IServiceRepository repo)
        {
            repository = repo;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult<IEnumerable<Business>> GetBusinesses()
        {
            IEnumerable<Business> businesses = repository.Business.
                Include(b => b.User).
                Include(b => b.Products).
                Include(b => b.Workdays);
            if (!businesses.Any()) return NoContent();
            foreach (var b in businesses)
            {
                b.User.Email = null;
                b.User.BirthDate = new DateTime(00,00,00);
                b.User.Passhash = null;
                b.User.Businesses = null;
                foreach (var w in b.Workdays) w.Business = null;
                foreach (var p in b.Products) p.Business = null;
                foreach (var f in b.Feedbacks) f.Business = null;
            }
            return Ok(businesses);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Business>> Business(long id)
        {
            Business business = await repository.Business.FirstOrDefaultAsync(b => b.BusinessID == id);
            if (business == null) return NotFound();
            return Ok(business);
        }

        [HttpDelete("{id}")]
        public async Task RemoveBusiness(long id)
        {
            await repository.RemoveBusinessAsync(new Business {BusinessID = id});
        }

        [HttpPost]
        public async Task<ActionResult> SaveBusiness(BusinessBindingTarget target)
        {
            await repository.SaveBusinessAsync(target.ToBusiness());
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateBusiness(Business business)
        {
            await repository.UpdateBusinessAsync(business);
            return Ok();
        }

    }
}
