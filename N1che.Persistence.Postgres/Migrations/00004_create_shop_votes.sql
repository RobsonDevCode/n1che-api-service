-- +goose Up
CREATE TABLE shop_votes (
    shop_id    uuid NOT NULL REFERENCES shops(id) ON DELETE CASCADE,
    user_id    text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (shop_id, user_id)
);

CREATE INDEX shop_votes_shop_idx ON shop_votes (shop_id);

-- +goose Down
DROP TABLE IF EXISTS shop_votes;
