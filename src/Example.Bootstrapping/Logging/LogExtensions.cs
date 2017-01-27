﻿using System;
using System.Collections.Concurrent;

namespace Example.Bootstrapping
{
    /// <summary>
    /// Extensions to help make logging awesome - this should be installed into the root namespace of your application
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Concurrent dictionary that ensures only one instance of a logger for a type.
        /// </summary>
        private static readonly ConcurrentDictionary<string, ILog> _dictionary = new ConcurrentDictionary<string, ILog>();
        /// <summary>
        /// Gets the logger for <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type to get the logger for.</param>
        /// <returns>Instance of a logger for the object.</returns>
        public static ILog Log<T>(this T type)
        {
            string objectName = GetFriendlyName(typeof(T));
            return Log(objectName);
        }

        private static string GetFriendlyName(Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].Name;
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        /// <summary>
        /// Gets the logger for the specified object name.
        /// </summary>
        /// <param name="objectName">Either use the fully qualified object name or the short. If used with Log&lt;T&gt;() you must use the fully qualified object name"/></param>
        /// <returns>Instance of a logger for the object.</returns>
        public static ILog Log(this string objectName)
        {
            return _dictionary.GetOrAdd(objectName, Example.Bootstrapping.Log.GetLoggerFor);
        }

        public static void Error(this ILog log, Exception exception, string format, params object[] args)
        {
            log.Error(() => String.Format(format, args), exception);
        }

        public static void Fatal(this ILog log, Exception exception, string format, params object[] args)
        {
            log.Fatal(() => String.Format(format, args), exception);
        }

        /// <summary>
        /// Formats and writes an information level log entry that looks
        /// like '============= Some message ============='.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Header(this ILog log, string format, params object[] args)
        {
            const string wrapping = " ============= ";
            var wrappedFormat = wrapping + format + wrapping;
            log.Info(wrappedFormat, args);
        }
    }
}