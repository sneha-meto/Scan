using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace Common;

public static class Log {
    private const string LOG_DATE_FORMAT = "dd-MM-yyyy HH:mm:ss.fff";
    private const string TEMPLATE = "[{Thread}] [{CallerNamespace}] {Tag}: {Message}";
    private static readonly LoggingLevelSwitch LOG_LEVEL = new();

    private static void SetLevel(LogEventLevel level) {
        LOG_LEVEL.MinimumLevel = level;
    }

    public static void Init(string logPath) {
        SetLevel(LogEventLevel.Debug);
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LOG_LEVEL)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true,
                outputTemplate:
                $"[{{Timestamp:{LOG_DATE_FORMAT}}}] {{Level:u}} {{Message:lj}}{{NewLine}}{{Exception}}"
            )
#if DEBUG
            .WriteTo.Console(
                outputTemplate:
                $"[{{Timestamp:{LOG_DATE_FORMAT}}}] {{Level:u}} {{Message:lj}}{{NewLine}}{{Exception}}"
            )
#endif
            .CreateLogger();
    }

    /// <summary>
    ///     Write a log event at the DEBUG level.
    /// </summary>
    /// <param name="tag">The TAG representing the class that is logged.</param>
    /// <param name="msg">The message to be logged.</param>
    public static void D(string tag, string msg) {
        (string thread, string @namespace) = GetLogParams();
        Serilog.Log.Debug(TEMPLATE, thread, @namespace, tag, msg);
    }

    /// <summary>
    ///     Write a log event at the INFORMATION level.
    /// </summary>
    /// <param name="tag">The TAG representing the class that is logged.</param>
    /// <param name="msg">The message to be logged.</param>
    public static void I(string tag, string msg) {
        (string thread, string @namespace) = GetLogParams();
        Serilog.Log.Information(TEMPLATE, thread, @namespace, tag, msg);
    }

    /// <summary>
    ///     Write a log event at the ERROR level.
    /// </summary>
    /// <param name="tag">The TAG representing the class that is logged.</param>
    /// <param name="msg">The message to be logged.</param>
    public static void E(string tag, string msg) {
        (string thread, string @namespace) = GetLogParams();
        Serilog.Log.Error(TEMPLATE, thread, @namespace, tag, msg);
    }

    /// <summary>
    ///     Write a log event at the WARNING level.
    /// </summary>
    /// <param name="tag">The TAG representing the class that is logged.</param>
    /// <param name="msg">The message to be logged.</param>
    public static void W(string tag, string msg) {
        (string thread, string @namespace) = GetLogParams();
        Serilog.Log.Warning(TEMPLATE, thread, @namespace, tag, msg);
    }

    /// <summary>
    ///     Write a log event at the FATAL level.
    /// </summary>
    /// <param name="tag">The TAG representing the class that is logged.</param>
    /// <param name="ex">The Exception to be logged</param>
    /// <param name="msg">The message to be logged.</param>
    public static void Fatal(string tag, Exception ex, string msg) {
        (string thread, string @namespace) = GetLogParams();
        Serilog.Log.Fatal(ex, TEMPLATE, thread, @namespace, tag, msg);
    }

    /// <summary>
    ///     Resets <see cref="P:Serilog.Log.Logger" /> to the default and disposes the original if possible
    /// </summary>
    public static void CloseAndFlush() {
        Serilog.Log.CloseAndFlush();
    }

    private static (string, string) GetLogParams() {
        return (Environment.CurrentManagedThreadId.ToString().PadLeft(2, '0'), GetCallerNamespace(2));
    }

    //TODO Replace with Type tag parameter
    private static string GetCallerNamespace(int frame) {
        return new StackTrace().GetFrame(frame + 1)?.GetMethod()?.ReflectedType?.Namespace ?? "Unknown.Namespace";
    }
}