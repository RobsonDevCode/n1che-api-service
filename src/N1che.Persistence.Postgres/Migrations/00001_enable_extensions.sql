-- +goose Up
CREATE EXTENSION IF NOT EXISTS postgis;
CREATE EXTENSION IF NOT EXISTS btree_gist;

-- +goose Down
-- Extensions are shared environment infrastructure (postgis is depended on by the
-- image's tiger/topology extensions) — a schema rollback must not tear them down.
