using CommandLine;
using ConsoleSnakeGame.Core;
using ConsoleSnakeGame.Core.Players;
using ConsoleSnakeGame.Options;
using Figgle;

Parser.Default.ParseArguments<PlayOptions, ExtraOptions>(args)
    .WithParsedAsync<PlayOptions>(LetsPlay).Result
    .WithParsed<ExtraOptions>(DoExtra);

async Task LetsPlay(PlayOptions options)
{
    Console.WriteLine(FiggleFonts.Standard.Render("Snake"));
    var settings = options.GetSettings();

    Console.Write("Let a bot play? [y/N]: ");

    Game game = Console.ReadKey().Key switch
    {
        ConsoleKey.Y => new SnakeGame<BotPlayer>(settings),
        _ => new SnakeGame<UserPlayer>(settings)
    };

    game.Start();

    try
    {
        await game;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("The game has been canceled :C");
    }
    catch (Exception e)
    {
        Console.WriteLine("Oops: " + e.Message);
    }
}

void DoExtra(ExtraOptions options)
{
    if (options.OPsFlag)
    {
        var output = ExtraOptions.GetOPsString();
        Console.WriteLine(output);
    }
}
