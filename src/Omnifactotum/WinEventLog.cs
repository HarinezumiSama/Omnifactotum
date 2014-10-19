using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides the simple interface to write to the Windows <b>Event Log</b>.
    /// </summary>
    public static class WinEventLog
    {
        #region Constants and Fields

        private const string LogName = "Application";

        private static volatile string _defaultSource;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the default source for the entries written
        ///     using <see cref="WinEventLog.Write(EventLogEntryType,string)"/>.
        /// </summary>
        public static string DefaultSource
        {
            [DebuggerStepThrough]
            get
            {
                return _defaultSource;
            }

            [DebuggerStepThrough]
            set
            {
                _defaultSource = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Writes the specified message to the <b>Windows Event Log</b>.
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
        public static void Write([NotNull] string source, EventLogEntryType type, [NotNull] string message)
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
        ///     Writes the message to the <b>Windows Event Log</b>, using the specified array of objects and
        ///     formatting information.
        /// </summary>
        /// <param name="source">
        ///     The source by which the application is registered on the specified computer.
        /// </param>
        /// <param name="type">
        ///     The type of the event log entry.
        /// </param>
        /// <param name="format">
        ///     A format string that contains zero or more format items, which correspond to objects in
        ///     the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        ///     An object array containing zero or more objects to format.
        /// </param>
        [StringFormatMethod("format")]
        public static void Write(
            [NotNull] string source,
            EventLogEntryType type,
            [NotNull] string format,
            [NotNull] params object[] args)
        {
            var message = string.Format(CultureInfo.InvariantCulture, format, args);
            Write(source, type, message);
        }

        /// <summary>
        ///     Writes the specified message to the <b>Windows Event Log</b>
        ///     using <see cref="WinEventLog.DefaultSource"/>.
        ///     If <see cref="WinEventLog.DefaultSource"/> is <b>null</b> or empty, the entry assembly name is used.
        /// </summary>
        /// <param name="type">
        ///     The type of the event log entry.
        /// </param>
        /// <param name="message">
        ///     The message to write to the event log.
        /// </param>
        public static void Write(EventLogEntryType type, [NotNull] string message)
        {
            var source = GetDefaultSource();
            Write(source, type, message);
        }

        /// <summary>
        ///     Writes the message to the <b>Windows Event Log</b>
        ///     using <see cref="WinEventLog.DefaultSource"/> and the specified array of objects and
        ///     formatting information.
        ///     If <see cref="WinEventLog.DefaultSource"/> is <b>null</b> or empty, the entry assembly name is used.
        /// </summary>
        /// <param name="type">
        ///     The type of the event log entry.
        /// </param>
        /// <param name="format">
        ///     A format string that contains zero or more format items, which correspond to objects in
        ///     the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        ///     An object array containing zero or more objects to format.
        /// </param>
        [StringFormatMethod("format")]
        public static void Write(
            EventLogEntryType type,
            [NotNull] string format,
            [NotNull] params object[] args)
        {
            var message = string.Format(CultureInfo.InvariantCulture, format, args);
            Write(type, message);
        }

        #endregion

        #region Private Methods

        [NotNull]
        private static string GetDefaultSource()
        {
            var result = _defaultSource.TrimSafely();
            if (!result.IsNullOrWhiteSpace())
            {
                return result;
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            result = assembly.GetName().Name;

            return result;
        }

        #endregion
    }
}