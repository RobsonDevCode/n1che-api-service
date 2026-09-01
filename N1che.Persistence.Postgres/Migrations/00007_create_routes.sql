-- +goose Up
CREATE TABLE routes (
    id                  uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    name                text NOT NULL,
    tag                 text NOT NULL,
    mode                text NOT NULL,
    niche               text NOT NULL REFERENCES niches(id),
    created_by_user_id  text NOT NULL,
    created_by_username text NOT NULL,
    anchor              geography(Point, 4326) NOT NULL,
    polyline            geography(LineString, 4326) NOT NULL,
    distance_meters     double precision NOT NULL,
    total_minutes       int  NOT NULL,
    vote_count          int  NOT NULL DEFAULT 0,
    created_at          timestamptz NOT NULL DEFAULT now(),
    updated_at          timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX routes_niche_anchor_gist ON routes USING GIST (niche, anchor);

-- +goose Down
DROP TABLE IF EXISTS routes;
