namespace EventConsumer.Models;

public record SpecialOfferEvent(
    long SequenceNumber,
    DateTimeOffset OccuredAt,
    string Name,
    object Content);
