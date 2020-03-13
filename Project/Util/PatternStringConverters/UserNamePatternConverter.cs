using System;
using System.IO;

namespace log4net.Util.PatternStringConverters
{
	/// <summary>
	/// Write the current threads username to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Write the current threads username to the output writer
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	internal sealed class UserNamePatternConverter : PatternConverter
	{
		/// <summary>
		/// The fully qualified type of the UserNamePatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(UserNamePatternConverter);

		/// <summary>
		/// Write the current threads username to the output
		/// </summary>
		/// <param name="writer">the writer to write to</param>
		/// <param name="state">null, state is not set</param>
		/// <remarks>
		/// <para>
		/// Write the current threads username to the output <paramref name="writer" />.
		/// </para>
		/// </remarks>
		protected override void Convert(TextWriter writer, object state)
		{
			writer.Write(SystemInfo.NotAvailableText);
		}
	}
}
