
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace TestCore
{
    class Program
    {
        // 获得一个日志器，捕捉当前Program类产生的错误
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        
        static void Main(string[] args)
        {
            // 最基本的配置
            BasicConfigurator.Configure(true, true);

            #region 其它各种配置方式

            // 配置log4net
            //log4net.Config.XmlConfigurator.Configure(null, "Configs\\log4net.config");
            //ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            //XmlConfigurator.Configure(repository, new FileInfo("Configs\\log4net.config"));
            //ILog log = LogManager.GetLogger(repository.Name, "NETCorelog4net");
            //log.Info(".NetCore log4net log");
            //log.Info("test log");
            //log.Error("error");
            //log.Info("linezero");

            // 读取appSettings段的logEnabled，检查是否允许开启日志
            var logEnabled = ConfigurationManager.AppSettings["logEnabled"];
            if (logEnabled == "1") // 开启日志
            {
                // 取得log4net配置文件
                //var appPath = AppDomain.CurrentDomain.BaseDirectory; // 取得应用程序集(exe)所在的目录
                //var config = ConfigurationManager.AppSettings["log4net"]; // log4net配置文件
                //var file = appPath + config;
                //var configFile = new System.IO.FileInfo(file);

                /*
                 * 配置方式一：采用特性配置 在AssemblyInfo.cs文件中设置：
                 */
                //[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Configs\\log4net.config", Watch = true)]

                /*
                 * 配置方式二： 采用默认工厂配置，比特性配置灵活，可以自定义配置文件路径
                 */
                //XmlConfigurator.Configure(configFile); //log4net.Config.XmlConfigurator.Configure(configFile);

                /*
                 * 配置方式三： 采用自定义工厂配置
                 */

                // 不要事先获得日志器，因为这是从默认工厂中获取到的:
                // //private static readonly ILog log = LogManager.GetLogger(typeof(Program));
                // 而应该只先声明:
                // private static ILog log = null;

                // 现在创建一个日志工厂,名称任意指定
                //ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");

                // 根据工厂设置配置文件
                //XmlConfigurator.Configure(repository, configFile);

                // 从工厂中获得日志器
                // log4net将type类型的名称(type.FullName)作为logger的名称，因此type不能重复。
                // 采用type.FullName作为日志器的名称，就能知道日志错误是由谁引发的。
                // log = LogManager.GetLogger(repository.Name, typeof(Program));

                /*
                 * 配置方式四： 将配置文件嵌入到程序集进行配置，如果不想看见配置文件，可以这样做。
                 */
                // 在csproj项目文件中添加：
                //<ItemGroup>
                //  <EmbeddedResource Include="Config\log4netEmbedManifest.config" />
                //  <None Include="Config\log4net.config">
                //    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
                //    <SubType>Designer</SubType>
                //  </None>
                //  <None Include="packages.config" />
                //</ItemGroup>
                // 
                // packages.config的内容：
                //<?xml version="1.0" encoding="utf-8"?>
                //<packages>
                //  <package id="log4net" version="2.0.3" targetFramework="net40" />
                //</packages>

            }

            #endregion

            try
            {
                throw new Exception("请重新登录系统"); // 模拟一个异常
            }
            catch (Exception e)
            {
                WriteLog(e, MethodBase.GetCurrentMethod().Name, nameof(Program));
            }
            
            Console.ReadLine();
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="e"></param>
        private static void WriteLog(Exception e, string methodName, string className, string extraData = "")
        {
            if (log.IsErrorEnabled)
            {
                var props = log4net.GlobalContext.Properties;
                props["source"] = $"来源: {e.Source}\r\n";
                props["class"] = $"对象: {className}\r\n";
                props["method"] = $"方法: {methodName}\r\n";
                props["message"] = $"消息: {e.Message}\r\n";
                props["detail"] = $"详情: {e.StackTrace.Trim()}\r\n";
                props["data"] = $"数据: {extraData}\r\n"; ;

                log.Debug(e.Message, e);
                log.Info(e.Message, e);
                log.Warn(e.Message, e);
                log.Error(e.Message, e);
                log.Fatal(e.Message, e);
            }
        }

    }
}






