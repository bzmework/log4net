namespace log4net
{
	/// <summary>
	/// Provides information about the environment the assembly has
	/// been built for.
	/// </summary>
	public sealed class AssemblyInfo
	{
		/// <summary>Version of the assembly</summary>
		public const string Version = "2.0.8";

		/// <summary>Version of the framework targeted</summary>
		public const decimal TargetFrameworkVersion = 1.0m;

		/// <summary>Type of framework targeted</summary>
		public const string TargetFramework = ".NET Core";

		/// <summary>Does it target a client profile?</summary>
		public const bool ClientProfile = false;

		/// <summary>
		/// Identifies the version and target for this assembly.
		/// </summary>
		public static string Info => string.Format("Apache log4net version {0} compiled for {1}{2} {3}", "2.0.8", ".NET Core", string.Empty, 1.0m);
	}
}
