using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BuberBreakfast.Controllers;

public class BuberBreakfast : ApiController
{
    private readonly IBreakfastService _breakfastService;

    public BuberBreakfast(IBreakfastService breakfastService)
    {
        _breakfastService = breakfastService;
    }

    [HttpPost("/breakfasts")] // !! leaving it empty is error
    public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);//Breakfast.Create(
            // Guid.NewGuid(),
            // request.Name,
            // request.Description,
            // request.StartDateTime,
            // request.EndDateTime,
            // // DateTime.UtcNow,
            // request.Savory,
            // request.Sweet
        // );

        if(requestToBreakfastResult.IsError)
        {
            return Problem(requestToBreakfastResult.Errors);
        }

        var breakfast = requestToBreakfastResult.Value;

        ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

        // var response = new BreakfastResponse(
        //     breakfast.Id,
        //     breakfast.Name,
        //     breakfast.Description,
        //     breakfast.StartDateTime,
        //     breakfast.EndDateTime,
        //     breakfast.LastModifiedDateTime,
        //     breakfast.Savory,
        //     breakfast.Sweet
        // );

        // if (createBreakfastResult.IsError)
        // {
        //     return Problem(createBreakfastResult.Errors);
        // }

        // return CreatedAsGetBreakfast(breakfast); //200      

        return createBreakfastResult.Match(
            created => CreatedAsGetBreakfast(breakfast),
            errors => Problem(errors)
        );
    }


    [HttpGet("/breakfasts/{id:guid}")]
    public IActionResult GetBreakfast(Guid id)
    {
        ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

        return getBreakfastResult.Match(
            breakfast => Ok(MapBreakfastResponse(breakfast)),
            errors => Problem(errors));
    }


    [HttpPut("/breakfasts/{id:guid}")]
    public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
    {
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(id,request);
        //     // id,
        //     request.Name,
        //     request.Description,
        //     request.StartDateTime,
        //     request.EndDateTime,
        //     // DateTime.UtcNow,
        //     request.Savory,
        //     request.Sweet,
        //     id
        // );

        if (requestToBreakfastResult.IsError)
        {
            return Problem(requestToBreakfastResult.Errors);
        }

        var breakfast = requestToBreakfastResult.Value;
        ErrorOr<UpsertedBreakfast> upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

        //TODO: return 201 if a new breakfast was created
        return upsertBreakfastResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAsGetBreakfast(breakfast) : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("/breakfasts/{id:guid}")]
    public IActionResult DeleteBreakfast(Guid id)
    {
        ErrorOr<Deleted> deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);
        return deleteBreakfastResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );
    }

    private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        return new BreakfastResponse(
                    breakfast.Id,
                    breakfast.Name,
                    breakfast.Description,
                    breakfast.StartDateTime,
                    breakfast.EndDateTime,
                    breakfast.LastModifiedDateTime,
                    breakfast.Savory,
                    breakfast.Sweet
                );
    }

    private CreatedAtActionResult CreatedAsGetBreakfast(Breakfast breakfast)
    {
        return CreatedAtAction(
                    actionName: nameof(GetBreakfast),
                    routeValues: new { id = breakfast.Id },
                    value: MapBreakfastResponse(breakfast));
    }
}