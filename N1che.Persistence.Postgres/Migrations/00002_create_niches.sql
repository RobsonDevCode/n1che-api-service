-- +goose Up
CREATE TABLE niches (
    id          text PRIMARY KEY,
    label       text NOT NULL,
    sub         text NOT NULL,
    description text NOT NULL
);

-- +goose Down
DROP TABLE IF EXISTS niches;
