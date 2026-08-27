using System;
using System.IO;
using System.Text;

namespace ZP2K9;

internal static class Program
{
	private static void Main(string[] args)
	{
		// TEMP DIAGNOSTIC (2026-08-24, "joiner shares host's data over WAN"
		// investigation): mirrors every Console.WriteLine the game ever does
		// - all the existing [Client]/[Host]/[Steam]/[LanOpResult]/
		// [GetPlayerOne] diagnostic lines added while chasing that bug,
		// plus anything else - to a plain log.txt file written next to the
		// game's .exe, so a friend testing over a real internet connection
		// can just send that file back instead of needing Visual Studio
		// installed to see the Output window. Overwritten fresh each run
		// (FileMode.Create) so there's never any doubt about which run a
		// log came from. Wrapped in try/catch so a failure to open the log
		// file (no write permission, disk full, etc.) never prevents the
		// game from launching - falls back to console-only output in that
		// case. Safe to remove once the WAN multiplayer investigation is
		// done and the TEMP DIAGNOSTIC Console.WriteLine calls elsewhere are
		// cleaned up too.
		try
		{
			string logPath = Path.Combine(AppContext.BaseDirectory, "log.txt");
			StreamWriter fileWriter = new StreamWriter(new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
			fileWriter.AutoFlush = true;
			Console.SetOut(new TeeTextWriter(Console.Out, fileWriter));
			Console.SetError(new TeeTextWriter(Console.Error, fileWriter));
			Console.WriteLine("[Log] Logging to " + logPath + " - session started " + DateTime.Now);
		}
		catch (Exception ex)
		{
			Console.WriteLine("[Log] Could not open log.txt for writing: " + ex.Message);
		}
		// TEMP DIAGNOSTIC, see the comment above - makes sure a truly
		// unhandled exception (one that would otherwise just crash the
		// process with nothing but a Windows error dialog) still gets its
		// full type/message/stack trace written to log.txt before the
		// process dies.
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Console.WriteLine("[Log] UNHANDLED EXCEPTION: " + e.ExceptionObject);
			}
			catch
			{
			}
		};
		using Game1 game = new Game1();
		game.Run();
	}
}

// TEMP DIAGNOSTIC, see the comment in Main() above. Forwards every Write/
// WriteLine call to two underlying writers at once (the real console, and
// the log file) so nothing needs to change at any of the existing
// Console.WriteLine call sites scattered across the codebase.
internal sealed class TeeTextWriter : TextWriter
{
	private readonly TextWriter _a;
	private readonly TextWriter _b;

	public TeeTextWriter(TextWriter a, TextWriter b)
	{
		_a = a;
		_b = b;
	}

	public override Encoding Encoding => _a.Encoding;

	public override void Write(char value)
	{
		_a.Write(value);
		_b.Write(value);
	}

	public override void Write(string value)
	{
		_a.Write(value);
		_b.Write(value);
	}

	public override void WriteLine(string value)
	{
		_a.WriteLine(value);
		_b.WriteLine(value);
	}

	public override void Flush()
	{
		_a.Flush();
		_b.Flush();
	}
}
