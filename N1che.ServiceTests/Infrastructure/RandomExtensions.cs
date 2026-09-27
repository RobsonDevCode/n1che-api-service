namespace N1che.ServiceTests.Infrastructure;

/// <summary>The values a scenario draws when it does not care which one it gets.</summary>
internal static class RandomExtensions
{
    private static readonly string[] SeededNiches =
    [
        NicheConstants.Goth,
        NicheConstants.Skater,
        NicheConstants.Vintage,
        NicheConstants.Streetwear,
        NicheConstants.Y2k,
        NicheConstants.Cottagecore,
        NicheConstants.WesternWear
    ];

    /// <summary>One of the niches the migrations seed, so it satisfies the foreign key to `niches`.</summary>
    internal static string Niche(this Random random) => SeededNiches[random.Next(SeededNiches.Length)];

    // Coordinates stay inside the WGS 84 bounds so the geography cast doesn't silently coerce them.
    internal static double Latitude(this Random random) => random.Next(-80, 80) + random.NextDouble();

    internal static double Longitude(this Random random) => random.Next(-170, 170) + random.NextDouble();

    internal static double Meters(this Random random) => random.Next(50, 2000) + random.NextDouble();

    internal static int Minutes(this Random random) => random.Next(1, 120);
}
