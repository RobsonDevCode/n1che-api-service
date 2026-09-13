using AutoFixture;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class ShopHoursEntityBuilder
{
    // Whole-minute trading hours, so the value survives a round trip through a Postgres `time` column.
    public static ShopHoursEntity BuildForToday(IFixture fixture, Guid shopId) =>
        fixture.Build<ShopHoursEntity>()
            .With(hours => hours.ShopId, shopId)
            .With(hours => hours.DayOfWeek, (int)DateTime.UtcNow.DayOfWeek)
            .With(hours => hours.OpenTime, new TimeOnly(Random.Shared.Next(6, 12), 0))
            .With(hours => hours.CloseTime, new TimeOnly(Random.Shared.Next(17, 23), 30))
            .Create();
}
