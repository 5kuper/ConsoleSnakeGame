using System.Diagnostics;

namespace Utilities.Terminal;

internal record KeyEventArgs(ConsoleKeyInfo Info);

internal class ConsoleInput
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private readonly Action<bool> _pauseSetter;
    private readonly Action _pauseToggle;

    private readonly Action _programTerminator;

    private bool _isTaskRunnig;
    private bool _isProgramCancelling;

    public ConsoleInput(Action<bool> pauseSetter, Action pauseToggle, Action programTerminator)
    {
        _pauseSetter = pauseSetter ?? throw new ArgumentNullException(nameof(pauseSetter));
        _pauseToggle = pauseToggle ?? throw new ArgumentNullException(nameof(pauseToggle));

        _programTerminator = programTerminator ?? throw new ArgumentNullException(nameof(programTerminator));
    }

    public event EventHandler<KeyEventArgs>? KeyUnhandled;

    public string ProgramName { get; set; } = "program";

    public async Task HandleAsync(CancellationToken cancellationToken = default)
    {
        if (_isTaskRunnig)
        {
            throw new InvalidOperationException("Cannot run a new task while an existing task is running.");
        }

        var task = Task.Run(() => Handle(cancellationToken), cancellationToken);
        _isTaskRunnig = true;
        await task;
    }

    private void Handle(CancellationToken cancellationToken)
    {
        Console.TreatControlCAsInput = true;

        while (!cancellationToken.IsCancellationRequested)
        {
            var ki = Console.ReadKey(!_isProgramCancelling);

            switch (ki)
            {
                case { Key: ConsoleKey.Y } when _isProgramCancelling:
                    _programTerminator();
                    Console.WriteLine();
                    break;

                case { Key: not ConsoleKey.Y } when _isProgramCancelling:
                    AvoidProgramCancellation();
                    break;

                case { Key: ConsoleKey.Enter or ConsoleKey.Spacebar }
                        when _stopwatch.ElapsedMilliseconds > 500:
                    _pauseToggle();
                    _stopwatch.Restart();
                    break;

                case { Key: ConsoleKey.Escape }:
                case { Modifiers: ConsoleModifiers.Control, Key: ConsoleKey.C }:
                    AskConfirmationOfProgramCancellation();
                    break;

                default:
                    OnKeyUnhandled(new(ki));
                    break;
            }
        }

        _isTaskRunnig = false;
        Console.TreatControlCAsInput = false;
        cancellationToken.ThrowIfCancellationRequested();
    }

    private void AskConfirmationOfProgramCancellation()
    {
        lock (Console.Out)
        {
            _pauseSetter(true);

            Console.Clear();
            Console.CursorVisible = true;
            Console.Write($"Are you sure you want to cancel the {ProgramName}? [y/N]: ");

            _isProgramCancelling = true;
        }
    }

    private void AvoidProgramCancellation()
    {
        Thread.Sleep(1000);
        Console.Clear();

        _isProgramCancelling = false;
        _pauseSetter(false);
    }

    protected virtual void OnKeyUnhandled(KeyEventArgs e)
    {
        KeyUnhandled?.Invoke(this, e);
    }
}
