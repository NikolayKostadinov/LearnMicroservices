﻿namespace SpecialOffers.Controllers
{
  using System;
  using System.Collections.Generic;
  using Data;
  using Microsoft.AspNetCore.Mvc;

  [Route("/offers")]
  public class SpecialOffersController : ControllerBase
  {
    private readonly IEventStore eventStore;
    private static readonly IDictionary<int, Offer> Offers = new Dictionary<int, Offer>();

    public SpecialOffersController(IEventStore eventStore) => this.eventStore = eventStore;

    [HttpGet("{id:int}")]
    public ActionResult<Offer> GetOffer(int id) =>
      Offers.ContainsKey(id)
        ? (ActionResult<Offer>) Ok(Offers[id])
        : NotFound();
    
    [HttpPost("")]
    public ActionResult<Offer> CreateOffer([FromBody] Offer offer)
    {
      if (offer == null)
        return BadRequest();
      var newUser = NewOffer(offer);
      return Created(new Uri($"/offers/{newUser.Id}", UriKind.Relative), newUser);
    }

    [HttpPut("{id:int}")]
    public Offer UpdateOffer(int id, [FromBody] Offer offer)
    {
      var offerWithId = offer with {Id = id};
      eventStore.RaiseEvent("SpecialOfferUpdated", new { OldOffer = Offers[id], NewOffer = offerWithId });
      return Offers[id] = offerWithId;
    }

    [HttpDelete("{id:int}")]
    public ActionResult DeleteOffer(int id)
    {
      eventStore.RaiseEvent("SpecialOfferRemoved", new { Offer = Offers[id] });
      Offers.Remove(id);
      return Ok();
    }

    private Offer NewOffer(Offer offer)
    {
      var offerId = Offers.Count;
      var newOffer = offer with {Id = offerId};
      eventStore.RaiseEvent("SpecialOfferCreated", newOffer);
      return Offers[offerId] = newOffer;
    }
  }

  public record Offer(string Description, int Id);
}
