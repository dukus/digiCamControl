using System;
using System.IO;
using System.Text;

namespace CameraControl.Core.TclScripting
{
	public delegate void OutputEventHandler (string message, bool newline);

	/// <summary>
	/// Redirect the console output for those threads that have called <see cref="Start"/> and have not yet called
	/// <see cref="Stop"/>.
	/// </summary>
	public class ThreadConsoleRedirect
	{
		public static object Start (OutputEventHandler handler)
		{
			lock (_lock)
			{
				if (_numHandlers == 0)
				{
					_defaultWriter = Console.Out;
					Console.SetOut(new ConsoleRedirector());
				}
				_numHandlers++;
			}
			_handler = handler;
			_line = "";
			return handler;
		}

		public static void Stop (object returnedByStart)
		{
			if ((object)_handler == returnedByStart)
			{
				_handler = null;
				lock (_lock)
				{
					_numHandlers--;
					if (_numHandlers == 0)
					{
						Console.SetOut(_defaultWriter);
						_defaultWriter = null;
					}
				}
			}
		}

		private static readonly object _lock = new object();
		private static int _numHandlers;
		private static TextWriter _defaultWriter;
		[ThreadStatic]
		private static OutputEventHandler _handler;
		[ThreadStatic]
		private static string _line = "";

		private class ConsoleRedirector : TextWriter
		{
			public override void Write (char value)
			{
				if (_handler is null)
				{
					// No handler configured or output from a thread without handler.
					// Write value to the default console.
					_defaultWriter?.Write(value);
				}
				else
				{
					if (value == '\n')
					{
						Write(_line);
						_line = "";
					}
					else
					{
						_line += value;
					}
				}
			}

			public override void Write (string value)
			{
				if (_handler is null)
				{
					(Console.Out == this ? _defaultWriter : Console.Out)?.Write(value);
				}
				else
				{
					_handler.Invoke(value, false);
				}
			}

			public override Encoding Encoding
			{
				get { return Encoding.ASCII; }
			}
		}
	}
}
