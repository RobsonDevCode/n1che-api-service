using Microsoft.Extensions.Logging;
using N1che.Domain.Exceptions;
using N1che.Domain.Extensions;
using N1che.Domain.Interfaces;
using N1che.Domain.Interfaces.Persistence.Writers.Shops;
using N1che.Domain.Interfaces.Services.Niches;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Services.Shops;

public sealed class ShopCreationService : IShopCreationService
{
    private readonly IGooglePlacesClient _googlePlacesClient;
    private readonly INicheRetrievalService _nicheRetrievalService;
    private readonly IShopsWriter _shopsWriter;
    private readonly IShopHoursWriter _shopHoursWriter;
    private readonly ITransactionScope _transactionScope;
    private readonly ILogger<ShopCreationService> _logger;

    public ShopCreationService(
        IGooglePlacesClient googlePlacesClient,
        INicheRetrievalService nicheRetrievalService,
        IShopsWriter shopsWriter,
        IShopHoursWriter shopHoursWriter,
        ITransactionScope transactionScope,
        ILogger<ShopCreationService> logger)
    {
        _googlePlacesClient = googlePlacesClient;
        _nicheRetrievalService = nicheRetrievalService;
        _shopsWriter = shopsWriter;
        _shopHoursWriter = shopHoursWriter;
        _transactionScope = transactionScope;
        _logger = logger;
    }

    public async Task<ShopModel> CreateAsync(CreateShopRequestModel request, CancellationToken cancellationToken)
    {
        await EnsureNichesAreRecognised(request.Niches, cancellationToken);

        var place = await _googlePlacesClient.GetPlaceDetails(request.GooglePlaceId, cancellationToken);
        if (place is null)
        {
            _logger.LogWarning("Google has no place {GooglePlaceId}", request.GooglePlaceId);
            throw new InvalidRequestException(EntityTypes.Place, request.GooglePlaceId);
        }

        if (place.IsPermanentlyClosed)
        {
            _logger.LogWarning("Google place {GooglePlaceId} is permanently closed", request.GooglePlaceId);
            throw new InvalidRequestException(EntityTypes.Place, request.GooglePlaceId);
        }

        var newShop = new NewShopModel
        {
            GooglePlaceId = request.GooglePlaceId,
            Name = place.Name,
            Address = place.Address,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            Niches = request.Niches,
            AddedByUserId = request.AddedByUserId,
            AddedByUsername = request.AddedByUsername,
        };

        var shop = await _transactionScope.ExecuteAsync(async token =>
        {
            var created = await _shopsWriter.Create(newShop, token);
            await _shopHoursWriter.Create(Guid.Parse(created.Id), place.OpeningHours, token);

            return created;
        }, cancellationToken);

        var todaysHours = place.OpeningHours
            .FirstOrDefault(hours => hours.DayOfWeek == DateTime.UtcNow.ToDayOfWeekIndex());

        return shop with { OpenTime = todaysHours?.OpenTime, CloseTime = todaysHours?.CloseTime };
    }

    private async Task EnsureNichesAreRecognised(IReadOnlyCollection<string> niches, CancellationToken cancellationToken)
    {
        var recognised = await _nicheRetrievalService.GetAllAsync(cancellationToken);
        var recognisedIds = recognised.Select(niche => niche.Id).ToHashSet(StringComparer.Ordinal);

        var unrecognised = niches.FirstOrDefault(niche => !recognisedIds.Contains(niche));
        if (unrecognised is null)
        {
            return;
        }

        _logger.LogWarning("Niche {Niche} is not recognised", unrecognised);

        throw new InvalidRequestException(EntityTypes.Niche, unrecognised);
    }
}
