-- +goose Up
CREATE TABLE shop_hours (
    id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    shop_id     uuid NOT NULL REFERENCES shops(id) ON DELETE CASCADE,
    day_of_week int  NOT NULL CHECK (day_of_week BETWEEN 0 AND 6),
    open_time   time NOT NULL,
    close_time  time NOT NULL
);

CREATE UNIQUE INDEX shop_hours_shop_day_idx ON shop_hours (shop_id, day_of_week);

-- +goose Down
DROP TABLE IF EXISTS shop_hours;
