﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestAPI.Infrastructure;
using RestAPI.Models;
using RestAPI.Models.BindingTargets;
using RestAPI.Models.Repositories;

namespace RestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {

        private IServiceRepository repository;
        private long claimedId;
        private IConfiguration configuration;

        public FeedbackController(IServiceRepository repo, IHttpContextAccessor accessor, IConfiguration config)
        {
            claimedId = long.Parse(accessor.HttpContext.User.Claims.FirstOrDefault(type => type.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
            repository = repo;
            configuration = config;
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{businessId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks(long businessId)
        {
            if (businessId < 1) return BadRequest();
            Business business = await repository.Business.FirstOrDefaultAsync(b => b.BusinessID == businessId);
            if (business == null)
            {
                return NotFound();
            }
            if (business.UserID == claimedId)
            {
                IEnumerable<Feedback> feedbacks = repository.Feedbacks.Where(f => f.BusinessID == businessId);
                foreach (var f in feedbacks)
                {
                    f.Business = null;
                }
                if (!feedbacks.Any()) return NoContent();
                return Ok(feedbacks);
            }
            return Unauthorized();
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveFeedback(FeedbackBindingTarget feedbackBindingTarget)
        {
            Feedback feedback = feedbackBindingTarget.ToFeedback();
            Business business = await repository.Business.FirstOrDefaultAsync(b => b.BusinessID == feedback.BusinessID);
            if (business == null) return NotFound();
            await repository.SaveFeedbackAsync(feedback);
            new Mailer(repository, configuration).SendMail(feedback);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("All/{businessId}")]
        public async Task<IActionResult> DeleteAllFeedbacks(long businessId)
        {
            if (businessId < 1) return BadRequest();
            Business business = await repository.Business.Include(b => b.Feedbacks).FirstOrDefaultAsync(b => b.BusinessID == businessId);
            IEnumerable<Feedback> feedbacks = business.Feedbacks;
            User user = business.User;
            if (business.UserID != claimedId)
            {
                return Unauthorized();
            }
            await repository.RemoveFeedbacksAsync(feedbacks);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("One/{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(long feedbackId)
        {
            if (feedbackId < 1) return BadRequest();
            Feedback feedback = await repository.Feedbacks.Include(f => f.Business).FirstOrDefaultAsync(f => f.ID == feedbackId);
            Business business = feedback.Business;
            if (business.UserID != claimedId)
            {
                return Unauthorized();
            }
            await repository.RemoveFeedbackAsync(feedback);
            return Ok();
        }

    }
}
