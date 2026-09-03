using System.Diagnostics;
using System.Text;

namespace DDoveFramework.Core
{
    public static class DDoveDebug
    {
        [Conditional("DEBUG")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("DEBUG")]
        public static void Log(string title, params (string key, object value)[] args)
        {
            UnityEngine.Debug.Log(Format(title, args));
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string title, params (string key, object value)[] args)
        {
            UnityEngine.Debug.LogWarning(Format(title, args));
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void LogError(string title, params (string key, object value)[] args)
        {
            UnityEngine.Debug.LogError(Format(title, args));
        }

        private static string Format(string title, (string key, object value)[] args)
        {
            var sb = new StringBuilder(64);
            if (!string.IsNullOrEmpty(title))
            {
                sb.Append('[').Append(title).Append(']');
            }

            if (args == null)
            {
                return sb.ToString();
            }

            for (var i = 0; i < args.Length; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(' ');
                }

                sb.Append('(').Append(args[i].key).Append(", ").Append(args[i].value).Append(')');
            }

            return sb.ToString();
        }
    }
}
