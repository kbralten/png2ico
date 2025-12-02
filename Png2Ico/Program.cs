using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Png2Ico;

class Program
{
    static void Main(string[] args)
    {
        // Parse options
        int size = 128;
        var argsList = new List<string>(args);
        int sizeIndex = argsList.IndexOf("--size");

        if (sizeIndex != -1)
        {
            if (sizeIndex + 1 < argsList.Count && int.TryParse(argsList[sizeIndex + 1], out int parsedSize) && parsedSize > 0)
            {
                size = parsedSize;
                // Remove --size and the value
                argsList.RemoveRange(sizeIndex, 2);
            }
            else
            {
                Console.WriteLine("Error: --size argument requires a valid positive integer.");
                return;
            }
        }

        if (argsList.Count < 1)
        {
            Console.WriteLine("Usage: Png2Ico <input.png> [output.ico] [--size <pixels>]");
            return;
        }

        string inputPath = argsList[0];
        string outputPath;

        if (argsList.Count >= 2)
        {
            outputPath = argsList[1];
        }
        else
        {
            outputPath = Path.ChangeExtension(inputPath, ".ico");
        }

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Error: Input file '{inputPath}' not found.");
            return;
        }

        try
        {
            using (Image image = Image.Load(inputPath))
            {
                Console.WriteLine($"Converting '{inputPath}' to '{outputPath}' with size {size}x{size}...");

                // Resize to specified size on short edge and crop to square
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(size, size),
                    Mode = ResizeMode.Crop
                }));
                
                using (var ms = new MemoryStream())
                {
                    // Save as PNG to memory stream to ensure it's valid PNG data
                    image.SaveAsPng(ms);
                    byte[] pngBytes = ms.ToArray();

                    using (var fs = new FileStream(outputPath, FileMode.Create))
                    using (var writer = new BinaryWriter(fs))
                    {
                        // 1. Write ICO Header
                        // Reserved (2 bytes)
                        writer.Write((short)0);
                        // Type (2 bytes, 1 = Icon, 2 = Cursor)
                        writer.Write((short)1);
                        // Count (2 bytes, number of images)
                        writer.Write((short)1);

                        // 2. Write Icon Directory Entry
                        // Width (1 byte, 0 means 256)
                        writer.Write((byte)(image.Width >= 256 ? 0 : image.Width));
                        // Height (1 byte, 0 means 256)
                        writer.Write((byte)(image.Height >= 256 ? 0 : image.Height));
                        // Color Count (1 byte, 0 if >= 8bpp)
                        writer.Write((byte)0);
                        // Reserved (1 byte)
                        writer.Write((byte)0);
                        // Planes (2 bytes, usually 1)
                        writer.Write((short)1);
                        // BitCount (2 bytes, usually 32 for RGBA)
                        writer.Write((short)32);
                        // SizeInBytes (4 bytes)
                        writer.Write(pngBytes.Length);
                        // FileOffset (4 bytes)
                        // Header (6) + 1 Entry (16) = 22
                        writer.Write(22);

                        // 3. Write Image Data
                        writer.Write(pngBytes);
                    }
                }
                
                Console.WriteLine("Conversion complete.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting file: {ex.Message}");
        }
    }
}