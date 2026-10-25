-- +goose Up
CREATE TABLE route_stops (
    route_id uuid NOT NULL REFERENCES routes(id) ON DELETE CASCADE,
    shop_id  uuid NOT NULL REFERENCES shops(id) ON DELETE RESTRICT,
    position int  NOT NULL,
    PRIMARY KEY (route_id, position)
);

CREATE INDEX route_stops_shop_idx ON route_stops (shop_id);

-- +goose Down
DROP TABLE IF EXISTS route_stops;
