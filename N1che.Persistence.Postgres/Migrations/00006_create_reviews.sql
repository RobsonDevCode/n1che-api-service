-- +goose Up
CREATE TABLE reviews (
    id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    shop_id      uuid NOT NULL REFERENCES shops(id) ON DELETE CASCADE,
    niche        text NOT NULL REFERENCES niches(id),
    user_id      text NOT NULL,
    username     text NOT NULL,
    body         text NOT NULL,
    update_count int  NOT NULL DEFAULT 0,
    created_at   timestamptz NOT NULL DEFAULT now(),
    updated_at   timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX reviews_shop_niche_idx ON reviews (shop_id, niche, created_at DESC);

-- +goose Down
DROP TABLE IF EXISTS reviews;
