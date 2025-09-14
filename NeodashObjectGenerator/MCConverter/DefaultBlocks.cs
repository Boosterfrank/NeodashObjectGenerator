using System.Numerics;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Minecraft;
using NeodashObjectGenerator.Minecraft.Blocks;

namespace NeodashObjectGenerator.MCConverter;

// Adds some predefined blocks to a block converter
public static class DefaultBlocks
{
    // Make a quick converter for a type with a default constructor
    public static Func<string, TagCompound, Block> Make<T>()
        where T : Block, new()
    {
        return (s, _) => new T {Name = s};
    }

    // Make a quick converter using a generation function
    public static Func<string, TagCompound, Block> Make<T>(Func<T> cons)
        where T : Block
    {
        return (_, _) => cons();
    }

    public static void AddBasicBlocks(this BlockConverter converter)
    {
        converter.Ignore("minecraft:air");
        converter.Ignore("minecraft:barrier");
        converter.Ignore("minecraft:bubble_column");
        converter.Ignore("minecraft:spruce_wall_sign");
        converter.Ignore("minecraft:spruce_door");
        converter.Ignore("minecraft:spruce_trapdoor");
        converter.Ignore("minecraft:black_stained_glass_pane");
        converter.Ignore("minecraft:command_block");
        converter.Ignore("minecraft:chain_command_block");
        converter.Ignore("minecraft:redstone_wire");
        converter.Ignore("minecraft:comparator");
        converter.Ignore("minecraft:repeater");
        converter.Ignore("minecraft:sticky_piston");
        converter.Ignore("minecraft:piston_head");
        converter.Ignore("minecraft:repeating_command_block");
        converter.Ignore("minecraft:chest");
        converter.Ignore("minecraft:water_cauldron");
        converter.Ignore("minecraft:red_carpet");
        converter.Ignore("minecraft:dark_oak_wall_sign");
        converter.Ignore("minecraft:potted_lily_of_the_valley");
        converter.Ignore("minecraft:lever");
        converter.Ignore("minecraft:stone_button");
        converter.Ignore("minecraft:torch");
        converter.Ignore("minecraft:wall_torch");
        converter.Ignore("minecraft:dispenser");
        converter.Ignore("minecraft:white_wall_banner");
        converter.Ignore("minecraft:birch_wall_sign");
        converter.Ignore("minecraft:trapped_chest");
        converter.Ignore("minecraft:ladder");
        converter.Ignore("minecraft:black_stained_glass");
        converter.Ignore("minecraft:birch_sign");
        converter.Ignore("minecraft:rail");
        converter.Add("minecraft:stone", FlatColorBlock.ForColor(Color.Rgb(120, 120, 120)));
        converter.Add("minecraft:bedrock", FlatColorBlock.ForColor(Color.Rgb(41, 41, 41)));
        converter.Add("minecraft:stone_bricks", FlatColorBlock.ForColor(Color.Rgb(122, 122, 122)));
        converter.Add("minecraft:ice", FlatColorBlock.ForColor(Color.Rgb(164, 192, 249)));
        converter.Add("minecraft:blue_ice", FlatColorBlock.ForColor(Color.Rgb(102, 147, 231)));
        converter.Add("minecraft:packed_ice", FlatColorBlock.ForColor(Color.Rgb(147, 185, 252)));
        converter.Add("minecraft:snow_block", FlatColorBlock.ForColor(Color.White));
        converter.Add("minecraft:glowstone", FlatColorBlock.ForColor(new Vector3(1.0f, 0.472605f, 0.0f), glowing: true));
        converter.Add("minecraft:sea_lantern", FlatColorBlock.ForColor(Color.Rgb(220, 228, 221), glowing: true));
        converter.Add("minecraft:magma_block", FlatColorBlock.ForColor(Color.Rgb(202, 128, 62), glowing: true));
        converter.Add("minecraft:note_block", FlatColorBlock.ForColor(Color.Rgb(89, 51, 34)));
        converter.Add("minecraft:snow", Snow.Read);
        converter.Add("minecraft:grass_block", ToppedBlock.Colors(Color.Rgb(79, 120, 71), Color.Rgb(90, 63, 44)));
        converter.Add("minecraft:dirt_path", ToppedBlock.Colors(Color.Rgb(150, 126, 71), Color.Rgb(90, 63, 44), true));
        converter.Add("minecraft:grass", Make<TallGrass>());
        converter.Add("minecraft:tall_grass", TallGrass.Read);
        converter.Add("minecraft:dirt", FlatColorBlock.ForColor(Color.Rgb(90, 63, 44)));
        converter.Add("minecraft:soul_sand", FlatColorBlock.ForColor(Color.Rgb(74, 55, 44), 14));
        converter.Add("minecraft:poppy", Make(() => new Flower(new Vector3(0.158114f, 0.445003f, 0.124264f))));
        converter.Add("minecraft:dandelion", Make(() => new Flower(new Vector3(1.0f, 0.729261f, 0.0f))));
        converter.Add("minecraft:oak_leaves", FlatColorBlock.ForColor(Color.Rgb(40, 79, 46)));
        converter.Add("minecraft:spruce_leaves", FlatColorBlock.ForColor(Color.Rgb(34, 66, 39)));
        converter.Add("minecraft:oak_log", FlatColorBlock.ForColor(Color.Rgb(84, 64, 10)));
        converter.Add("minecraft:oak_fence", Make(() => new Fence(Color.Rgb(147, 115, 67))));
        converter.Add("minecraft:spruce_log", FlatColorBlock.ForColor(Color.Rgb(15, 4, 0)));
        converter.Add("minecraft:spruce_planks", FlatColorBlock.ForColor(Color.Rgb(132, 90, 49)));
        converter.Add("minecraft:podzol", ToppedBlock.Colors(Color.Rgb(69, 45, 24), Color.Rgb(90, 63, 44)));
        converter.Add("minecraft:diamond_block", FlatColorBlock.ForColor(Color.Rgb(77, 189, 189)));
        converter.Add("minecraft:gold_block", FlatColorBlock.ForColor(Color.Rgb(230, 205, 83)));
        converter.Add("minecraft:copper_block", FlatColorBlock.ForColor(Color.Rgb(158, 117, 41)));
        converter.Add("minecraft:dark_oak_slab", Slab.ForColor(Color.Rgb(62, 31, 9)));
        converter.Add("minecraft:spruce_stairs", Stairs.ForColor(Color.Rgb(132, 90, 49)));
        converter.Add("minecraft:dark_oak_stairs", Stairs.ForColor(Color.Rgb(62, 31, 9)));
        converter.Add("minecraft:lantern", Lantern.Read);
    }
}