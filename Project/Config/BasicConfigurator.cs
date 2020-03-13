using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using static log4net.Appender.ManagedColoredConsoleAppender;

namespace log4net.Config
{
	/// <summary>
	/// Use this class to quickly configure a <see cref="T:log4net.Repository.Hierarchy.Hierarchy" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Allows very simple programmatic configuration of log4net.
	/// </para>
	/// <para>
	/// Only one appender can be configured using this configurator.
	/// The appender is set at the root of the hierarchy and all logging
	/// events will be delivered to that appender.
	/// </para>
	/// <para>
	/// Appenders can also implement the <see cref="T:log4net.Core.IOptionHandler" /> interface. Therefore
	/// they would require that the <see cref="M:log4net.Core.IOptionHandler.ActivateOptions()" /> method
	/// be called after the appenders properties have been configured.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public sealed class BasicConfigurator
	{
		/// <summary>
		/// The fully qualified type of the BasicConfigurator class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(BasicConfigurator);

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Config.BasicConfigurator" /> class. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Uses a private access modifier to prevent instantiation of this class.
		/// </para>
		/// </remarks>
		private BasicConfigurator()
		{
		}

		/// <summary>
		/// Initializes with a default configuration.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes the specified repository using a <see cref="T:log4net.Appender.ConsoleAppender" />
		/// that will write to <c>Console.Out</c>. The log messages are
		/// formatted using the <see cref="T:log4net.Layout.PatternLayout" /> layout object
		/// with the <see cref="F:log4net.Layout.PatternLayout.DetailConversionPattern" />
		/// layout style.
		/// </para>
		/// </remarks>
		public static ICollection Configure()
		{
			// 创建默认工厂
			ILoggerRepository repository;
			string repositoryName = "log4net-default-repository"; // 默认工厂
			if (LoggerManager.Exists(repositoryName))
			{
				repository = LoggerManager.GetRepository(repositoryName);
			}
			else
			{
				repository = LoggerManager.CreateRepository(repositoryName);
			}

			// 进行配置
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				PatternLayout patternLayout = new PatternLayout();
				patternLayout.ConversionPattern = "%timestamp [%thread] %level %logger %ndc - %message%newline";
				patternLayout.ActivateOptions();
				ConsoleAppender consoleAppender = new ConsoleAppender();
				consoleAppender.Layout = patternLayout;
				consoleAppender.ActivateOptions();
				InternalConfigure(repository, consoleAppender);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Initializes with a default configuration.
		/// </summary>
		/// <param name="useConsole">是否输出到控制台</param>
		/// <param name="useFile">是否输出到文件</param>
		/// <returns></returns>
		public static ICollection Configure(bool useConsole = false, bool useFile = true)
		{
			// 创建默认工厂
			ILoggerRepository repository;
			string repositoryName = "log4net-default-repository"; // 默认工厂
			if (LoggerManager.Exists(repositoryName))
			{
				repository = LoggerManager.GetRepository(repositoryName);
			}
			else
			{
				repository = LoggerManager.CreateRepository(repositoryName);
			}

			// 进行配置
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				// 日志输出格式, 各种参数设置参见PatternLayout.cs:
				// %m(message):    输出的日志消息;
				// %n(newline):    换行;
				// %d(datetime):   输出当前语句运行的时刻;
				// %r(runtime):    输出程序从运行到执行到当前语句时消耗的毫秒数;
				// %t(threadid):   当前语句所在的线程ID;
				// %p(priority):   日志的当前日志级别;
				// %c(class):      当前日志对象的名称;
				// %L:             输出语句所在的行号;
				// %F:             输出语句所在的文件名;
				// %-10:           表示最小长度为10，如果不够，则用空格填充;
				PatternLayout patternLayout = new PatternLayout();

				//patternLayout.ConversionPattern = "--------时间: %d, 耗时: %r(ms), 类型: %-5p --------%n对象: %c%n消息: %m%n%n";
				//patternLayout.ConversionPattern = "--------时间: %d, 耗时: %r(ms), 类型: %-5p --------%n对象: %c%n消息: %m%n详情: %exception%n%n";
				patternLayout.ConversionPattern = "--------时间: %d, 类型: %-5p --------%n%property{source}%property{class}%property{method}%property{message}%property{detail}%property{data}%n";
				patternLayout.ActivateOptions();

				#region 文件输出器

				RollingFileAppender fileAppender = null;
				if (useFile)
				{
					fileAppender = new RollingFileAppender();
					// 日志文件名, 
					// 如果RollingStyle为Composite或Date, 则这里一般设置成目录名, 文件名在DatePattern里设置;
					// 如果RollingStyle为其他滚动方式, 则必须设置成文件名
					fileAppender.File = "log\\";

					// 是否是静态的日志文件名, 即固定的日志文件名称, 
					// 如果你想要动态生成日志文件名，例如将RollingStyle设置成Date或Composite, 以让系统自动生成日志文件的名称,
					// 那么你必须将staticLogFileName设置成false
					fileAppender.StaticLogFileName = false;

					// 日志滚动方式，可设置成:
					// Size(按文件), 此时你应该将file的值设置成一个固定的名称, 例如: test.log 或 log\\test.log 或 c:\\log\\test.log;
					// Date(按日期), 此时你应该将file的值设置成一个目录或者空字符串, 如果设置成空字符串, 系统将把日志记录在当前应用程序所在的目录中;
					// Composite(按日期及文件)，默认为Composite
					fileAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;

					// 日志文件名格式, 
					// 当RollingStyle为Composite或Date, 在此处设置日志文件名的格式时, 
					// 固定不变的部分用单引号括起来, 其它部分则设置成日期格式
					fileAppender.DatePattern = "yyyyMMdd'.log'";

					// 日志记录是否追加到文件, 
					// 默认为true, 表示将记录追加到文件尾部; flase, 表示覆盖已有的日志文件记录
					fileAppender.AppendToFile = true;

					// 单个日志文件的最大尺寸, 
					// 可用的单位有: KB | MB | GB, 默认为字节(不带单位)
					fileAppender.MaximumFileSize = "2MB";

					//每日最多记录的日志文件个数
					fileAppender.MaxSizeRollBackups = 10;

					// 单个日志文件超限后日志备份方式, 默认值为 - 1,
					// 当日志文件超过MaximumFileSize大小时，系统根据CountDirection的值来备份日志文件:
					// (1)当此值设置成 > -1时, 则根据file里指定的文件名依次按0,1,2...递增创建日志备份, 直到数量等于MaxSizeRollBackups参数值为止，
					// 以后的日志记录则会根据maximumFileSize循环写入file, 当filesize > maximumFileSize, 进行一次新的循环写入时, 会将file记录写入备份日志, 并对备份日志进行重新编号;
					// (2)当此值设置成 <= -1时, 则根据file里指定的文件名依次按0,1,2...递增创建日志备份, 直到数量等于MaxSizeRollBackups参数值为止，
					// 以后的日志记录则会根据maximumFileSize循环写入file, 当filesize > maximumFileSize, 进行一次新的循环写入时, 不会将file记录写入备份日志, 即备份日志被固化不受影响-- >
					fileAppender.CountDirection = -1;

					fileAppender.Layout = patternLayout;
					fileAppender.ActivateOptions();
				}

				#endregion

				#region 控制台输出器

				//ConsoleAppender consoleAppender = null;
				//if (useConsole)
				//{
				//	consoleAppender = new ConsoleAppender();
				//	consoleAppender.Layout = patternLayout;
				//	consoleAppender.ActivateOptions();
				//}

				#endregion

				#region 颜色控制台输出器

				ManagedColoredConsoleAppender coloredConsoleAppender = null;
				if (useConsole)
				{
					coloredConsoleAppender = new ManagedColoredConsoleAppender();

					// 设置日志级别, 默认值为DEBUG, 
					// 级别由低到高依次为: ALL | DEBUG < INFO < WARN < ERROR < FATAL | OFF.其中:
					// ALL表示记录所有日志;
					// OFF表示不记录日志, 即关闭日志记录;
					// 其它则按级别记录，例如级别设置成WARN，则低于WARN级别的INFO和DEBUG日志将不会被记录, 其它依次类推.
					var debugMap = new LevelColors
					{
						ForeColor = ConsoleColor.White,
						Level = Level.Debug
					};

					var infoMap = new LevelColors
					{
						ForeColor = ConsoleColor.Green,
						Level = Level.Info
					};

					var warnMap = new LevelColors
					{
						ForeColor = ConsoleColor.Yellow,
						Level = Level.Warn
					};

					var errorMap = new LevelColors
					{
						ForeColor = ConsoleColor.Red,
						Level = Level.Error
					};

					var fatalMap = new LevelColors
					{
						ForeColor = ConsoleColor.Magenta,
						Level = Level.Fatal
					};

					debugMap.ActivateOptions();
					infoMap.ActivateOptions();
					warnMap.ActivateOptions();
					errorMap.ActivateOptions();
					fatalMap.ActivateOptions();
					coloredConsoleAppender.AddMapping(debugMap);
					coloredConsoleAppender.AddMapping(warnMap);
					coloredConsoleAppender.AddMapping(infoMap);
					coloredConsoleAppender.AddMapping(errorMap);
					coloredConsoleAppender.AddMapping(fatalMap);

					coloredConsoleAppender.Layout = patternLayout;
					coloredConsoleAppender.ActivateOptions();
				}

				#endregion

				var appenders = new List<IAppender>();
				if (useFile)
				{
					appenders.Add(fileAppender);
				}
				if (useConsole)
				{
					appenders.Add(coloredConsoleAppender);
				}

				if (appenders.Count > 0)
				{
					InternalConfigure(repository, appenders.ToArray());
				}
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}


		/// <summary>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> with a default configuration.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <remarks>
		/// <para>
		/// Initializes the specified repository using a <see cref="T:log4net.Appender.ConsoleAppender" />
		/// that will write to <c>Console.Out</c>. The log messages are
		/// formatted using the <see cref="T:log4net.Layout.PatternLayout" /> layout object
		/// with the <see cref="F:log4net.Layout.PatternLayout.DetailConversionPattern" />
		/// layout style.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				PatternLayout patternLayout = new PatternLayout();
				patternLayout.ConversionPattern = "%timestamp [%thread] %level %logger %ndc - %message%newline";
				patternLayout.ActivateOptions();
				ConsoleAppender consoleAppender = new ConsoleAppender();
				consoleAppender.Layout = patternLayout;
				consoleAppender.ActivateOptions();
				InternalConfigure(repository, consoleAppender);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appender.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="appender">The appender to use to log all logging events.</param>
		/// <remarks>
		/// <para>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appender.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, IAppender appender)
		{
			return Configure(repository, new IAppender[1]
			{
				appender
			});
		}

		/// <summary>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appenders.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="appenders">The appenders to use to log all logging events.</param>
		/// <remarks>
		/// <para>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appender.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, params IAppender[] appenders)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, appenders);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigure(ILoggerRepository repository, params IAppender[] appenders)
		{
			IBasicRepositoryConfigurator basicRepositoryConfigurator = repository as IBasicRepositoryConfigurator;
			if (basicRepositoryConfigurator != null)
			{
				basicRepositoryConfigurator.Configure(appenders);
			}
			else
			{
				LogLog.Warn(declaringType, "BasicConfigurator: Repository [" + repository + "] does not support the BasicConfigurator");
			}
		}
	}
}
