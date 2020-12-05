﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Cryptography;
using RestAPI.Models;
using RestAPI.Models.BindingTargets;
using RestAPI.Models.Repositories;

namespace RestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private IServiceRepository repository;
        private long claimedId;

        public BusinessController(IServiceRepository repo, IHttpContextAccessor accessor)
        {
            claimedId = long.Parse(accessor.HttpContext.User.Claims.FirstOrDefault(type => type.Value == ClaimTypes.NameIdentifier)?.Value ?? "0");
            repository = repo;
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("All")]
        public ActionResult<IEnumerable<Business>> GetBusinesses()
        {
            IEnumerable<Business> businesses = repository.Business.
                Include(b => b.User).
                Include(b => b.Products).
                Include(b => b.Workdays);
            if (!businesses.Any()) return NoContent();
            foreach (var b in businesses)
            {
                b.EliminateDepth();
            }
            return Ok(businesses);
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Filtered")]
        public ActionResult<IEnumerable<Business>> GetFilteredBusinesses(SearchEngine searchEngine)
        {
            IEnumerable<Business> filteredBusinesses = searchEngine.FilterBusinesses(repository);
            foreach (var b in filteredBusinesses)
            {
                b.EliminateDepth();
            }
            if (!filteredBusinesses.Any()) return NoContent();
;            return Ok(filteredBusinesses);
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Business>> Business(long id)
        {
            Business business = await repository.Business.
                Include(b => b.Workdays).Include(b => b.Products).Include(b => b.User).
                FirstOrDefaultAsync(b => b.BusinessID == id);
            if (business == null) return NotFound();
            business.EliminateDepth();
            return Ok(business);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveBusiness(string password, long id)
        {
            if (id < 1) return BadRequest();
            Business business = await repository.Business.FirstOrDefaultAsync(b => b.BusinessID == id);
            if (business == null)
            {
                return NotFound();
            }
            if (business.UserID != claimedId)
            {
                return Unauthorized();
            }
            User user = await repository.Users.FirstOrDefaultAsync(u => u.UserID == claimedId);
            if (new HashCalculator().IsGoodPass(user.Passhash, password))
            {
                await repository.RemoveBusinessAsync(business);
                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult> SaveBusiness(BusinessBindingTarget target)
        {
            await repository.SaveBusinessAsync(target.ToBusiness(claimedId));
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut]
        public async Task<ActionResult> UpdateBusiness(string password, Business business)
        {
            if (business.BusinessID < 1) return BadRequest();
            business.UserID = claimedId;
            business.User = null;
            Business targetBusiness = await repository.Business.Include(b => b.User).FirstOrDefaultAsync(b => b.BusinessID == business.BusinessID);
            if (targetBusiness == null)
            {
                return NotFound();
            }
            User user = targetBusiness.User;
            if (user.UserID != claimedId)
            {
                return Unauthorized();
            }
            if (new HashCalculator().IsGoodPass(user.Passhash, password))
            {
                await repository.UpdateBusinessAsync(business);
                return Ok();
            }
            return Unauthorized();
        }

    }
}