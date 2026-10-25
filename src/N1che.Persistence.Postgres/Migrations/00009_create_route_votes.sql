-- +goose Up
CREATE TABLE route_votes (
    route_id   uuid NOT NULL REFERENCES routes(id) ON DELETE CASCADE,
    user_id    text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (route_id, user_id)
);

CREATE INDEX route_votes_route_idx ON route_votes (route_id);

-- +goose Down
DROP TABLE IF EXISTS route_votes;
