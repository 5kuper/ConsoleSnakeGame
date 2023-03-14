using CommandLine;
using ConsoleSnakeGame.Core;

namespace ConsoleSnakeGame.Options
{
    [Verb("play", isDefault: true, HelpText = "Start the game.")]
    internal class PlayOptions
    {
        [Option("color", HelpText = "Color of the snake (green/cyan/yellow).")]
        public SnakeColor? SnakeColor { get; set; }

        [Option('c', "config", Default = "Config.json", HelpText = CHelpText)]
        public string? ConfigPath { get; set; }

        private const string CHelpText = "Path to a configuration file." +
            "\nIf the config is missing, a default one will be created.";

        [Option('o', "ops", Separator = ':', HelpText = OHelpText)]
        public IEnumerable<string>? OPs { get; set; }

        private const string OHelpText = "Colon-separated list of obstacle placement variants, " +
            "e.g. \'-o dispersion\' or \'-o corners:cross\'." +
            "\n(Enter \'do --ls-ops\' to print a list of all variants)";

        public SnakeGame.Settings GetSettings()
        {
            string? config = null;

            if (File.Exists(ConfigPath))
            {
                config = File.ReadAllText(ConfigPath);
            }
            else if (ConfigPath is not null)
            {
                File.WriteAllText(ConfigPath, SnakeGame.Settings.GetDefaultJsonConfig());
            }

            return SnakeGame.Settings.Construct(config, OPs, SnakeColor);
        }
    }
}
