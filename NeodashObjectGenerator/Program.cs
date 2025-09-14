using System.Numerics;
using NeodashObjectGenerator.MCConverter;

var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var worldFolder = Path.Join(appData, @"Roaming\PrismLauncher\instances\1.21.8 alt\minecraft\saves\Snow Hill Ice Track\region");
var outputFile = Path.Join(localAppData, @"Runway\Saved\external\Levels\Unnamed.csv");
var worldRegion = new Region(new Location(-209, 4, -177), new Location(-15, 50, 20));
var blockOrigin = new Location(0, -60, 0);
var worldOrigin = new Vector3(0, 0, 0);

Console.WriteLine("Initializing");
var converter = new BlockConverter();
converter.AddBasicBlocks();

var world = new SimpleWorld();
world.SetupOffset(new Location(), worldOrigin);
world.Header = @"metadata,difficultyMultiplier,0.8,associatedSong,None,isCampaign,0,isLevelValidated,0
VectorVariables,heightFogColor,0.0,0.062517,0.117336,sunColor,1.0,0.0,0.0,domeColor,1.0,0.0,0.0,ScalarVariables,sunIntensity,0.5,
deleterPoints,timePoints,posPoints,";

Console.WriteLine("Reading blocks...");
var reader = new WorldReader(worldFolder);
var blocks = reader.ReadAllBlocks(worldRegion);
foreach (var (location, tag) in blocks)
{
    world[location] = converter.Convert(tag);
}

// --- MODIFIED SECTION ---
Console.WriteLine("Generating meshes...");
var components = world.GenerateMeshes();

Console.WriteLine("Generate output file...");
world.GenerateFile(outputFile, components);
// --- END MODIFIED SECTION ---

Console.WriteLine("Done");