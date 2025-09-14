using System.Numerics;
using NeodashObjectGenerator.MCConverter;
using NeodashObjectGenerator.Minecraft.Blocks;

var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var worldFolder = Path.Join(appData, @"Roaming\PrismLauncher\instances\1.21.8 alt\minecraft\saves\Snow Hill Ice Track\region");
var outputFile = Path.Join(localAppData, @"Runway\Saved\external\Levels\Unnamed.csv");
var worldRegion = new Region(new Location(-209, 4, -177), new Location(-15, 50, 20));
var blockOrigin = new Location(-137, 5, -94);
var worldOrigin = new Vector3(18806, -8498, 463);


// var gen = new StructureGenerator(new Vector3(25178, -16414, 292), new Location(0, 1, 0));
// gen.Add(@"multiCube2,25178.0,-16414.0,1317.434448,0.0,0.0,0.0,0.156457,0.156457,0.264841,ScalarMaterialParameters,glowIntensity,0.0,VectorMaterialParameters,BaseColor,0.10462,0.10462,0.10462,
// multiCube2,25178.0,-16414.0,1090.223511,0.0,0.0,0.0,0.3,0.3,0.4,ScalarMaterialParameters,glowIntensity,0.0,VectorMaterialParameters,BaseColor,0.048607,0.048607,0.048607,
// multiCube2,25178.0,-16414.0,1189.341675,0.0,0.0,0.0,0.223352,0.223352,0.331736,ScalarMaterialParameters,glowIntensity,0.0,VectorMaterialParameters,BaseColor,0.078768,0.078768,0.078768,
// multiCube,25178.0,-16414.0,1090.223511,89.999954,0.0,0.0,0.307346,0.3,0.3,ScalarMaterialParameters,BandThickness,0.695424,beatModifiesBandThickness,0.134072,VectorMaterialParameters,GlowColor,0.345904,0.105958,0.007168,
// multiCube,25178.0,-16414.0,1090.223511,89.999939,0.0,89.999947,0.305947,0.3,0.3,ScalarMaterialParameters,BandThickness,0.695424,beatModifiesBandThickness,0.134072,VectorMaterialParameters,GlowColor,0.345904,0.105958,0.007168,");
// Console.WriteLine(gen.ToString());
//
// return;

Console.WriteLine("Initializing");
var converter = new BlockConverter();
converter.AddBasicBlocks();

var world = new SimpleWorld();
world.SetupOffset(new Location(), worldOrigin);
world.Header = @"metadata,difficultyMultiplier,0.8,associatedSong,None,isCampaign,0,isLevelValidated,0
VectorVariables,heightFogColor,0.0,0.062517,0.117336,sunColor,1.0,0.0,0.0,domeColor,1.0,0.0,0.0,ScalarVariables,sunIntensity,0.5,
deleterPoints,timePoints,posPoints,";

// world[blockOrigin] = converter.Get("minecraft:stone");
// world[blockOrigin + new Location(0, 1, 0)] = converter.Get("minecraft:grass_block");
// world[blockOrigin + new Location(0, 3, 0)] = converter.Get("minecraft:stone");
// world[blockOrigin + new Location(0, 2, 0)] = converter.Convert(Block.MakeTag("minecraft:lantern", "hanging=true"));
// world[blockOrigin + new Location(-1, 0, 0)] = converter.Get("minecraft:grass_block");
// world[blockOrigin + new Location(-2, 0, 0)] = converter.Get("minecraft:podzol");
// world.GenerateFile(outputFile);
//
// return;

Console.WriteLine("Reading blocks...");
var reader = new WorldReader(worldFolder);
var blocks = reader.ReadAllBlocks(worldRegion);
foreach (var (location, tag) in blocks)
{
    world[location] = converter.Convert(tag);
}

Console.WriteLine("Generate output file...");
world.GenerateFile(outputFile);

Console.WriteLine("Done");