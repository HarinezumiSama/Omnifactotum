﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using static Omnifactotum.FormattableStringFactotum;

//// TODO [HarinezumiSama] Create a wrapper interface for System.Console to make possible writing unit tests

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the colored console trace listener.
    /// </summary>
    public sealed class ColoredConsoleTraceListener : TextWriterTraceListener
    {
        private const TraceEventType DefaultEventType = TraceEventType.Verbose;

        private readonly object _syncLock;
        private ConsoleColor _errorColor;
        private ConsoleColor _warningColor;
        private ConsoleColor _messageColor;
        private ConsoleColor _informationColor;
        private uint _colorChangedCount;
        private ConsoleColor? _originalForegroundColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColoredConsoleTraceListener"/> class.
        /// </summary>
        public ColoredConsoleTraceListener()
            : base(CreateSynchronizedWrapper(Console.Out))
        {
            // In .NET Framework, a synchronized TextWriter locks on itself
            _syncLock = Writer;

            ErrorColor = ConsoleColor.Red;
            WarningColor = ConsoleColor.Yellow;
            InformationColor = ConsoleColor.DarkGreen;
            MessageColor = ConsoleColor.DarkYellow;
        }

        /// <summary>
        ///     Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the trace listener is thread safe; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsThreadSafe => true;

        /// <summary>
        ///     Gets or sets the color used for error messages.
        /// </summary>
        public ConsoleColor ErrorColor
        {
            [DebuggerNonUserCode]
            get
            {
                lock (_syncLock)
                {
                    return _errorColor;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (_syncLock)
                {
                    _errorColor = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the color used for warning messages.
        /// </summary>
        public ConsoleColor WarningColor
        {
            [DebuggerNonUserCode]
            get
            {
                lock (_syncLock)
                {
                    return _warningColor;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (_syncLock)
                {
                    _warningColor = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the color used for information messages.
        /// </summary>
        public ConsoleColor InformationColor
        {
            [DebuggerNonUserCode]
            get
            {
                lock (_syncLock)
                {
                    return _informationColor;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (_syncLock)
                {
                    _informationColor = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the color used for non-specific messages.
        /// </summary>
        public ConsoleColor MessageColor
        {
            [DebuggerNonUserCode]
            get
            {
                lock (_syncLock)
                {
                    return _messageColor;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (_syncLock)
                {
                    _messageColor = value;
                }
            }
        }

        /// <summary>
        ///     Closes the <see cref="TextWriterTraceListener.Writer"/> so that it no longer receives tracing or
        ///     debugging output.
        /// </summary>
        public override void Close()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Emits an error message to the listener you create when you implement
        ///     the <see cref="TraceListener"/> class.
        /// </summary>
        /// <param name="message">
        ///     A message to emit.
        /// </param>
        public override void Fail(string message)
        {
            lock (_syncLock)
            {
                //// TODO [HarinezumiSama] Use disposable instead of try/finally (?)

                ChangeColor(TraceEventType.Error);
                try
                {
                    base.Fail(message);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Emits an error message and a detailed error message to the listener you create when
        ///     you implement the <see cref="TraceListener"/> class.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public override void Fail(string message, string detailMessage)
        {
            lock (_syncLock)
            {
                ChangeColor(TraceEventType.Error);
                try
                {
                    base.Fail(message, detailMessage);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and
        ///     stack trace information.
        /// </param>
        /// <param name="source">
        ///     A name used to identify the output, typically the name of the application that generated
        ///     the trace event.
        /// </param>
        /// <param name="eventType">
        ///     One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">
        ///     A numeric identifier for the event.
        /// </param>
        /// <param name="data">
        ///     The trace data to emit.
        /// </param>
        public override void TraceData(
            TraceEventCache eventCache,
            string source,
            TraceEventType eventType,
            int id,
            object data)
        {
            lock (_syncLock)
            {
                ChangeColor(eventType);
                try
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes trace information, an array of data objects and event information to
        ///     the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and
        ///     stack trace information.
        /// </param>
        /// <param name="source">
        ///     A name used to identify the output, typically the name of the application that generated
        ///     the trace event.
        /// </param>
        /// <param name="eventType">
        ///     One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">
        ///     A numeric identifier for the event.
        /// </param>
        /// <param name="data">
        ///     An array of objects to emit as data.
        /// </param>
        public override void TraceData(
            TraceEventCache eventCache,
            string source,
            TraceEventType eventType,
            int id,
            params object[] data)
        {
            lock (_syncLock)
            {
                ChangeColor(eventType);
                try
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes trace and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID,
        ///     thread ID, and stack trace information.</param>
        /// <param name="source">
        ///     A name used to identify the output, typically the name of the application that generated
        ///     the trace event.
        /// </param>
        /// <param name="eventType">
        ///     One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">
        ///     A numeric identifier for the event.
        /// </param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            lock (_syncLock)
            {
                ChangeColor(eventType);
                try
                {
                    base.TraceEvent(eventCache, source, eventType, id);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and
        ///     stack trace information.
        /// </param>
        /// <param name="source">
        ///     A name used to identify the output, typically the name of the application that generated
        ///     the trace event.
        /// </param>
        /// <param name="eventType">
        ///     One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">
        ///     A numeric identifier for the event.
        /// </param>
        /// <param name="message">
        ///     A message to write.
        /// </param>
        public override void TraceEvent(
            TraceEventCache eventCache,
            string source,
            TraceEventType eventType,
            int id,
            string message)
        {
            lock (_syncLock)
            {
                ChangeColor(eventType);
                try
                {
                    base.TraceEvent(eventCache, source, eventType, id, message);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes trace information, a formatted array of objects and event information to
        ///     the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and
        ///     stack trace information.
        /// </param>
        /// <param name="source">
        ///     A name used to identify the output, typically the name of the application that generated
        ///     the trace event.
        /// </param>
        /// <param name="eventType">
        ///     One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">
        ///     A numeric identifier for the event.
        /// </param>
        /// <param name="format">
        ///     A format string that contains zero or more format items, which correspond to objects in
        ///     the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        ///     An object array containing zero or more objects to format.
        /// </param>
        public override void TraceEvent(
            TraceEventCache eventCache,
            string source,
            TraceEventType eventType,
            int id,
            string format,
            params object[] args)
        {
            lock (_syncLock)
            {
                ChangeColor(eventType);
                try
                {
                    base.TraceEvent(eventCache, source, eventType, id, format, args);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes trace information, a message, a related activity identity and event information to
        ///     the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and
        ///     stack trace information.
        /// </param>
        /// <param name="source">
        ///     A name used to identify the output, typically the name of the application that generated
        ///     the trace event.
        /// </param>
        /// <param name="id">
        ///     A numeric identifier for the event.
        /// </param>
        /// <param name="message">
        ///     A message to write.
        /// </param>
        /// <param name="relatedActivityId">
        ///     A <see cref="Guid"/> object identifying a related activity.
        /// </param>
        public override void TraceTransfer(
            TraceEventCache eventCache,
            string source,
            int id,
            string message,
            Guid relatedActivityId)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.TraceTransfer(eventCache, source, id, message, relatedActivityId);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes a message to this instance's <see cref="TextWriterTraceListener.Writer"/>.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.Write(message);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes a message to this instance's <see cref="TextWriterTraceListener.Writer"/> followed by
        ///     a line terminator. The default line terminator is a carriage return followed by a line feed (\r\n).
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.WriteLine(message);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes the value of the object's <see cref="Object.ToString"/> method to the listener you create
        ///     when you implement the <see cref="TraceListener"/> class.
        /// </summary>
        /// <param name="o">
        ///     An <see cref="System.Object"/> whose fully qualified class name you want to write.
        /// </param>
        public override void Write(object o)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.Write(o);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes a category name and a message to the listener you create when
        ///     you implement the <see cref="TraceListener"/> class.
        /// </summary>
        /// <param name="message">
        ///     A message to write.
        /// </param>
        /// <param name="category">
        ///     A category name used to organize the output.
        /// </param>
        public override void Write(string message, string category)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.Write(message, category);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes a category name and the value of the object's <see cref="Object.ToString"/> method to
        ///     the listener you create when you implement the <see cref="TraceListener"/> class.
        /// </summary>
        /// <param name="o">
        ///     An <see cref="Object"/> whose fully qualified class name you want to write.
        /// </param>
        /// <param name="category">
        ///     A category name used to organize the output.
        /// </param>
        public override void Write(object o, string category)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.Write(o, category);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes the value of the object's <see cref="Object.ToString"/> method to the listener you create
        ///     when you implement the <see cref="TraceListener"/> class, followed by a line terminator.
        /// </summary>
        /// <param name="o">
        ///     An <see cref="Object"/> whose fully qualified class name you want to write.
        /// </param>
        public override void WriteLine(object o)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.WriteLine(o);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes a category name and a message to the listener you create when you implement
        ///     the <see cref="TraceListener"/> class, followed by a line terminator.
        /// </summary>
        /// <param name="message">
        ///     A message to write.
        /// </param>
        /// <param name="category">
        ///     A category name used to organize the output.
        /// </param>
        public override void WriteLine(string message, string category)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.WriteLine(message, category);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes a category name and the value of the object's <see cref="Object.ToString"/> method to
        ///     the listener you create when you implement the <see cref="TraceListener"/> class,
        ///     followed by a line terminator.
        /// </summary>
        /// <param name="o">
        ///     An <see cref="Object"/> whose fully qualified class name you want to write.
        /// </param>
        /// <param name="category">
        ///     A category name used to organize the output.
        /// </param>
        public override void WriteLine(object o, string category)
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.WriteLine(o, category);
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        /// <summary>
        ///     Writes the indent to the listener you create when you implement this class, and resets
        ///     the <see cref="TraceListener.NeedIndent"/> property to <c>false</c>.
        /// </summary>
        protected override void WriteIndent()
        {
            lock (_syncLock)
            {
                ChangeColor(DefaultEventType);
                try
                {
                    base.WriteIndent();
                }
                finally
                {
                    ResetColor();
                }
            }
        }

        private static TextWriter CreateSynchronizedWrapper(TextWriter textWriter)
        {
            return TextWriter.Synchronized(textWriter);
        }

        private void ChangeColorInternal(TraceEventType eventType)
        {
            if (_originalForegroundColor.HasValue)
            {
                throw new InvalidOperationException(
                    AsInvariant($@"Internal logic error ({nameof(_colorChangedCount)} = {_colorChangedCount}, {
                        nameof(_originalForegroundColor)} = {_originalForegroundColor})."));
            }

            _originalForegroundColor = Console.ForegroundColor;

            ConsoleColor color;

            //// ReSharper disable once SwitchStatementMissingSomeCases - By design
            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    color = ErrorColor;
                    break;

                case TraceEventType.Warning:
                    color = WarningColor;
                    break;

                case TraceEventType.Information:
                    color = InformationColor;
                    break;

                default:
                    color = MessageColor;
                    break;
            }

            Console.ForegroundColor = color;
        }

        private void ChangeColor(TraceEventType eventType)
        {
#if !NET5_0
            RuntimeHelpers.PrepareConstrainedRegions();
#endif
            try
            {
                // Nothing to do
            }
            finally
            {
                if (_colorChangedCount == 0)
                {
                    ChangeColorInternal(eventType);
                }

                checked
                {
                    _colorChangedCount++;
                }
            }
        }

        private void ResetColor()
        {
#if !NET5_0
            RuntimeHelpers.PrepareConstrainedRegions();
#endif
            try
            {
                // Nothing to do
            }
            finally
            {
                checked
                {
                    _colorChangedCount--;
                }

                if (_colorChangedCount == 0)
                {
                    if (!_originalForegroundColor.HasValue)
                    {
                        throw new InvalidOperationException("Internal logic error.");
                    }

                    Console.ForegroundColor = _originalForegroundColor.Value;
                    _originalForegroundColor = null;
                }
            }
        }
    }
}