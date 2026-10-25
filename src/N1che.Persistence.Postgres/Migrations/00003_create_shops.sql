-- +goose Up
CREATE TABLE shops (
    id                uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    google_place_id   text NOT NULL UNIQUE,
    name              text NOT NULL,
    niches            text[] NOT NULL,
    address           text NOT NULL,
    location          geography(Point, 4326) NOT NULL,
    added_by_user_id  text NOT NULL,
    added_by_username text NOT NULL,
    vote_count        int  NOT NULL DEFAULT 0,
    place_status      text NOT NULL DEFAULT 'operational',
    last_validated_at timestamptz,
    created_at        timestamptz NOT NULL DEFAULT now(),
    updated_at        timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX shops_niches_gin    ON shops USING GIN  (niches);
CREATE INDEX shops_location_gist ON shops USING GIST (location);

-- +goose Down
DROP TABLE IF EXISTS shops;
