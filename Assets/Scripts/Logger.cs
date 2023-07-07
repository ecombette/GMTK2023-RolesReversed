using System.Diagnostics;

public class Logger
{
    [Conditional("ENABLE_LOGS")]
    public static void Log(string message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [Conditional("ENABLE_LOGS")]
    public static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogWarning(string message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogWarning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogError(string message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(message);
    }
}
