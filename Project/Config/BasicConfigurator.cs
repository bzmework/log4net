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
			// ����Ĭ�Ϲ���
			ILoggerRepository repository;
			string repositoryName = "log4net-default-repository"; // Ĭ�Ϲ���
			if (LoggerManager.Exists(repositoryName))
			{
				repository = LoggerManager.GetRepository(repositoryName);
			}
			else
			{
				repository = LoggerManager.CreateRepository(repositoryName);
			}

			// ��������
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
		/// <param name="useConsole">�Ƿ����������̨</param>
		/// <param name="useFile">�Ƿ�������ļ�</param>
		/// <returns></returns>
		public static ICollection Configure(bool useConsole = false, bool useFile = true)
		{
			// ����Ĭ�Ϲ���
			ILoggerRepository repository;
			string repositoryName = "log4net-default-repository"; // Ĭ�Ϲ���
			if (LoggerManager.Exists(repositoryName))
			{
				repository = LoggerManager.GetRepository(repositoryName);
			}
			else
			{
				repository = LoggerManager.CreateRepository(repositoryName);
			}

			// ��������
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				// ��־�����ʽ, ���ֲ������òμ�PatternLayout.cs:
				// %m(message):    �������־��Ϣ;
				// %n(newline):    ����;
				// %d(datetime):   �����ǰ������е�ʱ��;
				// %r(runtime):    �����������е�ִ�е���ǰ���ʱ���ĵĺ�����;
				// %t(threadid):   ��ǰ������ڵ��߳�ID;
				// %p(priority):   ��־�ĵ�ǰ��־����;
				// %c(class):      ��ǰ��־���������;
				// %L:             ���������ڵ��к�;
				// %F:             ���������ڵ��ļ���;
				// %-10:           ��ʾ��С����Ϊ10��������������ÿո����;
				PatternLayout patternLayout = new PatternLayout();

				//patternLayout.ConversionPattern = "--------ʱ��: %d, ��ʱ: %r(ms), ����: %-5p --------%n����: %c%n��Ϣ: %m%n%n";
				//patternLayout.ConversionPattern = "--------ʱ��: %d, ��ʱ: %r(ms), ����: %-5p --------%n����: %c%n��Ϣ: %m%n����: %exception%n%n";
				patternLayout.ConversionPattern = "--------ʱ��: %d, ����: %-5p --------%n%property{source}%property{class}%property{method}%property{message}%property{detail}%property{data}%n";
				patternLayout.ActivateOptions();

				#region �ļ������

				RollingFileAppender fileAppender = null;
				if (useFile)
				{
					fileAppender = new RollingFileAppender();
					// ��־�ļ���, 
					// ���RollingStyleΪComposite��Date, ������һ�����ó�Ŀ¼��, �ļ�����DatePattern������;
					// ���RollingStyleΪ����������ʽ, ��������ó��ļ���
					fileAppender.File = "log\\";

					// �Ƿ��Ǿ�̬����־�ļ���, ���̶�����־�ļ�����, 
					// �������Ҫ��̬������־�ļ��������罫RollingStyle���ó�Date��Composite, ����ϵͳ�Զ�������־�ļ�������,
					// ��ô����뽫staticLogFileName���ó�false
					fileAppender.StaticLogFileName = false;

					// ��־������ʽ�������ó�:
					// Size(���ļ�), ��ʱ��Ӧ�ý�file��ֵ���ó�һ���̶�������, ����: test.log �� log\\test.log �� c:\\log\\test.log;
					// Date(������), ��ʱ��Ӧ�ý�file��ֵ���ó�һ��Ŀ¼���߿��ַ���, ������óɿ��ַ���, ϵͳ������־��¼�ڵ�ǰӦ�ó������ڵ�Ŀ¼��;
					// Composite(�����ڼ��ļ�)��Ĭ��ΪComposite
					fileAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;

					// ��־�ļ�����ʽ, 
					// ��RollingStyleΪComposite��Date, �ڴ˴�������־�ļ����ĸ�ʽʱ, 
					// �̶�����Ĳ����õ�����������, �������������ó����ڸ�ʽ
					fileAppender.DatePattern = "yyyyMMdd'.log'";

					// ��־��¼�Ƿ�׷�ӵ��ļ�, 
					// Ĭ��Ϊtrue, ��ʾ����¼׷�ӵ��ļ�β��; flase, ��ʾ�������е���־�ļ���¼
					fileAppender.AppendToFile = true;

					// ������־�ļ������ߴ�, 
					// ���õĵ�λ��: KB | MB | GB, Ĭ��Ϊ�ֽ�(������λ)
					fileAppender.MaximumFileSize = "2MB";

					//ÿ������¼����־�ļ�����
					fileAppender.MaxSizeRollBackups = 10;

					// ������־�ļ����޺���־���ݷ�ʽ, Ĭ��ֵΪ - 1,
					// ����־�ļ�����MaximumFileSize��Сʱ��ϵͳ����CountDirection��ֵ��������־�ļ�:
					// (1)����ֵ���ó� > -1ʱ, �����file��ָ�����ļ������ΰ�0,1,2...����������־����, ֱ����������MaxSizeRollBackups����ֵΪֹ��
					// �Ժ����־��¼������maximumFileSizeѭ��д��file, ��filesize > maximumFileSize, ����һ���µ�ѭ��д��ʱ, �Ὣfile��¼д�뱸����־, ���Ա�����־�������±��;
					// (2)����ֵ���ó� <= -1ʱ, �����file��ָ�����ļ������ΰ�0,1,2...����������־����, ֱ����������MaxSizeRollBackups����ֵΪֹ��
					// �Ժ����־��¼������maximumFileSizeѭ��д��file, ��filesize > maximumFileSize, ����һ���µ�ѭ��д��ʱ, ���Ὣfile��¼д�뱸����־, ��������־���̻�����Ӱ��-- >
					fileAppender.CountDirection = -1;

					fileAppender.Layout = patternLayout;
					fileAppender.ActivateOptions();
				}

				#endregion

				#region ����̨�����

				//ConsoleAppender consoleAppender = null;
				//if (useConsole)
				//{
				//	consoleAppender = new ConsoleAppender();
				//	consoleAppender.Layout = patternLayout;
				//	consoleAppender.ActivateOptions();
				//}

				#endregion

				#region ��ɫ����̨�����

				ManagedColoredConsoleAppender coloredConsoleAppender = null;
				if (useConsole)
				{
					coloredConsoleAppender = new ManagedColoredConsoleAppender();

					// ������־����, Ĭ��ֵΪDEBUG, 
					// �����ɵ͵�������Ϊ: ALL | DEBUG < INFO < WARN < ERROR < FATAL | OFF.����:
					// ALL��ʾ��¼������־;
					// OFF��ʾ����¼��־, ���ر���־��¼;
					// �����򰴼����¼�����缶�����ó�WARN�������WARN�����INFO��DEBUG��־�����ᱻ��¼, ������������.
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
