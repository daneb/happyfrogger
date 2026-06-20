using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using HappyFrog.Models;

namespace HappyFrog;

public class ImageProcessor
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly ImageOptions _options;
    private readonly string _outputPath;

    public ImageProcessor(ImageOptions options, string outputPath)
    {
        _options = options;
        _outputPath = outputPath;
    }

    public void ProcessImages()
    {
        if (!_options.Enabled) return;

        if (string.IsNullOrEmpty(_options.SourcePath) || !Directory.Exists(_options.SourcePath))
        {
            Console.WriteLine($"  Warning: images source path not found: {_options.SourcePath}");
            return;
        }

        var destDir = Path.Combine(_outputPath, _options.OutputSubPath);
        Directory.CreateDirectory(destDir);

        var files = Directory.GetFiles(_options.SourcePath, "*", SearchOption.AllDirectories)
            .Where(f => SupportedExtensions.Contains(Path.GetExtension(f)))
            .ToList();

        int optimized = 0, skipped = 0;

        foreach (var file in files)
        {
            var relative = Path.GetRelativePath(_options.SourcePath, file);
            var dest = Path.Combine(destDir, relative);
            var sameFile = Path.GetFullPath(file) == Path.GetFullPath(dest);

            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

            try
            {
                using var image = Image.Load(file);

                if (image.Width <= _options.MaxWidth)
                {
                    if (!sameFile) File.Copy(file, dest, overwrite: true);
                    skipped++;
                    continue;
                }

                image.Mutate(x => x.Resize(_options.MaxWidth, 0));

                if (sameFile)
                {
                    // Write to a temp file first to avoid clobbering the open source file
                    var tmp = dest + ".tmp";
                    Save(image, tmp, Path.GetExtension(dest));
                    File.Move(tmp, dest, overwrite: true);
                }
                else
                {
                    Save(image, dest);
                }

                optimized++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: could not process {relative}: {ex.Message}");
                if (!sameFile) File.Copy(file, dest, overwrite: true);
                skipped++;
            }
        }

        Console.WriteLine($"  Images: {optimized} resized, {skipped} copied as-is ({files.Count} total)");
    }

    private void Save(Image image, string dest, string? ext = null)
    {
        ext = (ext ?? Path.GetExtension(dest)).ToLowerInvariant();
        switch (ext)
        {
            case ".jpg":
            case ".jpeg":
                image.Save(dest, new JpegEncoder { Quality = _options.Quality });
                break;
            case ".png":
                image.Save(dest, new PngEncoder());
                break;
            case ".webp":
                image.Save(dest, new WebpEncoder { Quality = _options.Quality });
                break;
            default:
                image.Save(dest);
                break;
        }
    }
}
