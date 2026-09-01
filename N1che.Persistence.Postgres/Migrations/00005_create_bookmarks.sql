-- +goose Up
CREATE TABLE bookmarks (
    shop_id    uuid NOT NULL REFERENCES shops(id) ON DELETE CASCADE,
    user_id    text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (shop_id, user_id)
);

CREATE INDEX bookmarks_user_id_idx ON bookmarks (user_id);

-- +goose Down
DROP TABLE IF EXISTS bookmarks;
