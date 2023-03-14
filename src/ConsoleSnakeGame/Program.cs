using CommandLine;
using ConsoleSnakeGame.Core;
using ConsoleSnakeGame.Core.Players;
using ConsoleSnakeGame.Options;
using Figgle;

try
{
    Parser.Default.ParseArguments<PlayOptions, ExtraOptions>(args)
        .WithParsedAsync<PlayOptions>(LetsPlay).Result
        .WithParsed<ExtraOptions>(DoExtra);
}
catch (AggregateException e)
{
    Console.WriteLine("\nOops: " +
        string.Join(' ', e.InnerExceptions.Select(ex => ex.Message)));
}
catch (Exception e)
{
    Console.WriteLine("\nOops: " + e.Message);
}

async Task LetsPlay(PlayOptions options)
{
    var settings = options.GetSettings();
    Console.WriteLine(FiggleFonts.Standard.Render("ConSnake"));

    Console.Write("Let a bot play? [y/N]: ");

    Game game = Console.ReadKey().Key switch
    {
        ConsoleKey.Y => new SnakeGame<BotPlayer>(settings),
        _ => new SnakeGame<UserPlayer>(settings)
    };

    Game.Result? result = null;
    beginning: game.Start();

    try
    {
        result = await game;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("The game has been canceled :C");
        return;
    }

    Console.WriteLine($"\n\n{result.Status}. Press any key to continue...");
    Console.CursorVisible = true;

    lock (Console.In)
    {
        Console.Write("Do you want to retry? [Y/n]: ");

        if (Console.ReadKey().Key is not ConsoleKey.N)
            goto beginning;
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
