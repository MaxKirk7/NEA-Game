using System;
using Serilog;

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
try
{
    Log.Information("MonoGame application started."); // Example log messae
    using var game = new NEAGame.Game1();
    game.Run();
}
catch (Exception ex)
{
    Log.Error(ex.Message);
    Log.Error(ex.StackTrace);
    Log.CloseAndFlush();
}
