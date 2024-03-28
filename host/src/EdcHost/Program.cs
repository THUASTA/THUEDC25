using System.Text.Json;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace EdcHost;

class Program
{
    const string SerilogTemplate = "[{@t:HH:mm:ss} {@l:u3}] {#if Component is not null}{Component,-13} {#end}{@m}\n{@x}";

    static void Main()
    {
        // Setup logger using default settings before calling dotenv.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Config config = LoadConfig();

        SetupLogging(config.LoggingLevel);

        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        Log.Information($"EDCHost version: {version.Major}.{version.Minor}.{version.Build}");

        SetupAndRunEdcHost(config);

        // Wait forever
        Task.Delay(-1).Wait();
    }

    static void SetupAndRunEdcHost(Config config)
    {
        IEdcHost edcHost = new EdcHost(config);

        edcHost.Start();
    }

    static Config LoadConfig()
    {
        Config config = new();
        string path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        if (!File.Exists(path))
        {
            Log.Warning($"Config file not found at {path}, creating default config file...");
            File.WriteAllText(path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            try
            {
                config = JsonSerializer.Deserialize<Config>(File.ReadAllText(path))!;
            }
            catch (JsonException)
            {
                Log.Error($"Error parsing config file at {path}");

#if DEBUG
                throw;
#else
                Log.Information($"Using default config.");
#endif
            }
        }

        return config;
    }

    static void SetupLogging(string loggingLevelString)
    {
        // Configure Serilog
        Log.Logger = loggingLevelString switch
        {
            "Verbose" => new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),
            "Debug" => new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),
            "Information" => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),
            "Warning" => new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),
            "Error" => new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),
            "Fatal" => new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .WriteTo.Console(new ExpressionTemplate(SerilogTemplate, theme: TemplateTheme.Literate))
                .CreateLogger(),
            _ => throw new ArgumentOutOfRangeException(nameof(loggingLevelString), loggingLevelString, "invalid logging level")
        };

        // Configure Fleck logging
        ILogger fleckLogger = Log.Logger.ForContext("Component", "Fleck");
        Fleck.FleckLog.LogAction = (level, message, ex) =>
        {
            switch (level)
            {
                case Fleck.LogLevel.Debug:
                    fleckLogger.Debug(message, ex);
                    break;
                case Fleck.LogLevel.Info:
                    fleckLogger.Information(message, ex);
                    break;
                case Fleck.LogLevel.Warn:
                    fleckLogger.Warning(message, ex);
                    break;
                case Fleck.LogLevel.Error:
                    fleckLogger.Error(message, ex);
                    break;
            }
        };

        Log.Information($"logging level set to {loggingLevelString}");
    }
}
