﻿using Cuest.Data.App;
using Cuest.Dtos.Activities;
using Microsoft.AspNetCore.Mvc;
using static Cuest.Dtos.Activities.ActivityDto;
using Cuest.Models.Activities;

namespace Cuest.Controllers.Activities
{
    // Controllers/ActivitiesController.cs
    [ApiController]
    [Route("api/activities")]
    public class ActivitiesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ActivitiesController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<ActivityDetailsDto>> Create([FromBody] CreateActivityDto dto)
        {
            var place = await _db.Places.FindAsync(dto.PlaceId);
            if (place == null) return NotFound(new { error = "Place not found" });

            var act = new Activity { PlaceId = dto.PlaceId, Type = dto.Type.Trim(), Notes = dto.Notes };
            _db.Activities.Add(act);
            await _db.SaveChangesAsync();

            return Ok(new ActivityDetailsDto(act.Id, act.PlaceId, act.Type, act.Notes, act.CreatedUtc));
        }
    }

}
