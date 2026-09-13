using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Routes;
using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Extensions.Routes;

namespace N1che.Persistence.Postgres.Postgres.Readers.Routes;

public sealed class RouteReader : IRoutesReader
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public RouteReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearby(RoutesFilterModel filterModel, CancellationToken cancellationToken)
    {
        // The CTE picks the routes first so the stop aggregate only ever runs for the rows that survive the LIMIT.
        // Stops are aggregated per route rather than joined flat so the polyline ships once, not once per stop.
        // Niche is a plain equality rather than an optional OR so the planner can use it as the leading
        // column of routes_niche_anchor_gist.
        const string sql =
            """
            WITH nearby_routes AS (
                SELECT routes.id,
                       routes.name,
                       routes.tag,
                       routes.mode,
                       routes.niche,
                       routes.created_by_user_id,
                       routes.created_by_username,
                       routes.polyline,
                       routes.distance_meters,
                       routes.total_minutes,
                       routes.vote_count,
                       routes.created_at
                FROM routes
                WHERE ST_DWithin(routes.anchor, ST_MakePoint(@Longitude, @Latitude)::geography, @RadiusMeters)
                AND routes.niche = @Niche
                ORDER BY routes.vote_count DESC, routes.created_at DESC
                LIMIT @Limit
            )
            SELECT nearby_routes.id,
                   nearby_routes.name,
                   nearby_routes.tag,
                   nearby_routes.mode,
                   nearby_routes.niche,
                   nearby_routes.created_by_user_id,
                   nearby_routes.created_by_username,
                   ST_AsGeoJSON(nearby_routes.polyline) AS polyline_geo_json,
                   nearby_routes.distance_meters,
                   nearby_routes.total_minutes,
                   nearby_routes.vote_count,
                   nearby_routes.created_at,
                   COALESCE(stops.stops_json, '[]'::json) AS stops_json
            FROM nearby_routes
            LEFT JOIN LATERAL (
                SELECT json_agg(json_build_object(
                           'id', shops.id,
                           'name', shops.name,
                           'address', shops.address,
                           'latitude', ST_Y(shops.location::geometry),
                           'longitude', ST_X(shops.location::geometry),
                           'placeStatus', shops.place_status,
                           'position', route_stops.position)
                       ORDER BY route_stops.position) AS stops_json
                FROM route_stops
                JOIN shops ON shops.id = route_stops.shop_id
                WHERE route_stops.route_id = nearby_routes.id
            ) stops ON true
            ORDER BY nearby_routes.vote_count DESC, nearby_routes.created_at DESC
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            filterModel.Latitude,
            filterModel.Longitude,
            filterModel.RadiusMeters,
            filterModel.Niche,
            filterModel.Limit,
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<RouteCompositeEntity>(command);

        return entities.Select(entity => entity.ToDomainModel()).ToArray();
    }
}
