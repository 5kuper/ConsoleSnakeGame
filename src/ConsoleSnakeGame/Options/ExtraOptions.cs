using CommandLine;
using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements;

namespace ConsoleSnakeGame.Options
{
    [Verb("do", HelpText = "Perform extra action.")]
    internal class ExtraOptions
    {
        [Option("ls-ops", Group = "Actions", HelpText = "List all obstacle placement variants.")]
        public bool OPsFlag { get; set; }

        public static string GetOPsString()
        {
            var names = OPAttribute.Items.Value.Select(i => i.Instance.Name);
            return string.Join(", ", names.OrderBy(n => n)) + '.';
        }
    }
}
