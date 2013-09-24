using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//// Namespace is intentionally named so in order to simplify usage of the contained class

// ReSharper disable CheckNamespace
namespace System.Diagnostics

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Provides the simple interface to write to the Windows <b>Event Log</b>.
    /// </summary>
    public static class WinEventLog
    {
        #region Constants and Fields

        private const string LogName = "Application";

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the default source for the entries written
        ///     using <see cref="WinEventLog.Write(EventLogEntryType,string)"/>.
        /// </summary>
        public static string DefaultSource
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Writes the specified message to the Windows Event Log.
        /// </summary>
        /// <param name="source">
        ///     The source by which the application is registered on the specified computer.
        /// </param>
        /// <param name="type">
        ///     The type of the event log entry.
        /// </param>
        /// <param name="message">
        ///     The message to write to the event log.
        /// </param>
        public static void Write(string source, EventLogEntryType type, string message)
        {
            #region Argument Check

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException(
                    @"The value can be neither empty or whitespace-only string nor null.",
                    "source");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            #endregion

            try
            {
                if (!EventLog.SourceExists(source))
                {
                    try
                    {
                        EventLog.CreateEventSource(source, LogName);
                    }
                    catch (ArgumentException)
                    {
                        // The event source might already be created by another thread or process
                    }
                }

                // Win32Exception may be thrown if the event log is full.
                EventLog.WriteEntry(source, message, type);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }

                // Suppressing any exception as writing to the log should not cause any new unhandled exception
            }
        }

        /// <summary>
        ///     Writes the specified message to the Windows Event Log using <see cref="WinEventLog.DefaultSource"/>.
        ///     If <see cref="WinEventLog.DefaultSource"/> is <b>null</b> or empty, the entry assembly name is used.
        /// </summary>
        /// <param name="type">
        ///     The type of the event log entry.
        /// </param>
        /// <param name="message">
        ///     The message to write to the event log.
        /// </param>
        public static void Write(EventLogEntryType type, string message)
        {
            var source = GetDefaultSource();
            Write(source, type, message);
        }

        #endregion

        #region Private Methods

        private static string GetDefaultSource()
        {
            var result = DefaultSource.TrimSafely();
            if (result.IsNullOrEmpty())
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                result = assembly.GetName().Name;
            }

            return result;
        }

        #endregion
    }
}