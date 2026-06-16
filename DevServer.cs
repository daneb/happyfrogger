using System.Net;
using HappyFrog.Models;

namespace HappyFrog;

public class DevServer
{
    private readonly HappyFrogConfig _config;
    private readonly int _port;

    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".html"] = "text/html; charset=utf-8",
        [".css"]  = "text/css",
        [".js"]   = "application/javascript",
        [".json"] = "application/json",
        [".xml"]  = "application/xml",
        [".txt"]  = "text/plain",
        [".png"]  = "image/png",
        [".jpg"]  = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".gif"]  = "image/gif",
        [".svg"]  = "image/svg+xml",
        [".ico"]  = "image/x-icon",
        [".woff"]  = "font/woff",
        [".woff2"] = "font/woff2",
    };

    public DevServer(HappyFrogConfig config, int port = 4000)
    {
        _config = config;
        _port = port;
    }

    public async Task StartAsync(Func<Task> rebuildAction, CancellationToken cancellationToken)
    {
        var watchPaths = new List<string> { _config.MarkdownFilesPath };
        foreach (var book in _config.Books)
        {
            if (Directory.Exists(book.Path))
                watchPaths.Add(book.Path);
        }

        var watchers = watchPaths
            .Where(Directory.Exists)
            .Select(path => CreateWatcher(path, rebuildAction))
            .ToList();

        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{_port}/");
        listener.Start();

        Console.WriteLine($"  Serving at http://localhost:{_port}");
        Console.WriteLine("  Watching for changes. Press Ctrl+C to stop.\n");

        cancellationToken.Register(() =>
        {
            listener.Stop();
            foreach (var w in watchers) w.Dispose();
        });

        while (!cancellationToken.IsCancellationRequested)
        {
            HttpListenerContext ctx;
            try
            {
                ctx = await listener.GetContextAsync();
            }
            catch (HttpListenerException)
            {
                break;
            }

            _ = Task.Run(() => HandleRequest(ctx), CancellationToken.None);
        }
    }

    private static FileSystemWatcher CreateWatcher(string path, Func<Task> rebuildAction)
    {
        var watcher = new FileSystemWatcher(path, "*.md")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        // Debounce rapid saves (e.g. editor writing temp files)
        var throttle = new System.Timers.Timer(500) { AutoReset = false };
        throttle.Elapsed += async (_, _) =>
        {
            Console.WriteLine("  File changed — rebuilding...");
            try
            {
                await rebuildAction();
                Console.WriteLine("  Rebuild complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Rebuild failed: {ex.Message}");
            }
        };

        void Restart() { throttle.Stop(); throttle.Start(); }
        watcher.Changed += (_, _) => Restart();
        watcher.Created += (_, _) => Restart();
        watcher.Deleted += (_, _) => Restart();
        return watcher;
    }

    private void HandleRequest(HttpListenerContext ctx)
    {
        var urlPath = ctx.Request.Url?.LocalPath ?? "/";
        if (urlPath.EndsWith("/")) urlPath += "index.html";

        var filePath = Path.Combine(
            Path.GetFullPath(_config.OutputPath),
            urlPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(filePath))
        {
            var notFound = System.Text.Encoding.UTF8.GetBytes(
                "<html><body><h1>404 — Not Found</h1></body></html>");
            ctx.Response.StatusCode = 404;
            ctx.Response.ContentType = "text/html; charset=utf-8";
            ctx.Response.OutputStream.Write(notFound);
            ctx.Response.Close();
            return;
        }

        var ext = Path.GetExtension(filePath);
        ctx.Response.ContentType = MimeTypes.GetValueOrDefault(ext, "application/octet-stream");
        ctx.Response.StatusCode = 200;

        using var file = File.OpenRead(filePath);
        file.CopyTo(ctx.Response.OutputStream);
        ctx.Response.Close();
    }
}
