-- +goose Up
INSERT INTO niches (id, label, sub, description) VALUES
    ('goth',        'Goth',       'dark',         'Dark aesthetics, velvet, silver hardware'),
    ('oldmoney',    'Old Money',  'quiet luxury', 'Tailored cuts, cashmere, quiet luxury'),
    ('skater',      'Skater',     'streetwise',   'Baggy fits, graphic tees, low-tops'),
    ('streetwear',  'Streetwear', 'culture',      'Limited drops, hoodies, sneaker culture'),
    ('cottagecore', 'Cottage',    'soft',         'Floral prints, linen, handmade pieces'),
    ('y2k',         'Y2K',        'retro-future', 'Low rise, chrome, butterfly clips'),
    ('techwear',    'Techwear',   'utility',      'Utility, waterproof, tactical fits'),
    ('vintage',     'Vintage',    'archive',      'Deadstock, 80s/90s, thrift finds');

-- +goose Down
DELETE FROM niches WHERE id IN (
    'goth', 'oldmoney', 'skater', 'streetwear',
    'cottagecore', 'y2k', 'techwear', 'vintage'
);
