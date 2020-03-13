# log4net
log4net日志管理系统(.Net Core). log4net for .Net Core. 

缘起：最近搭建Asp.Net Core WebApi底层的时候，引入日志系统的时候准备采用log4net，发现其几年没有更新了，也没有看到官方有说明，
支持.Net Core的版本也是很早的，于是从NuGet上下载了.NET Core版本的log4net，并根据官方资料对其进行了梳理，
翻译整理结果参见“Log4Net配置.docx”。   
   
毋庸置疑log4net是优秀的，正如其官方所说是经过时间检验的，在奉行快餐文化的今天希望您能花点时间静下心来审视一下log4net，
或许你会惊讶的发现："哦，原来log4net是可以不需要log4net.config配置文件的！"，"它的设计思想很好，但有的地方也不是很完美！"。   
   
下图是log4net的架构图，根据官方介绍，我猜测log4net从java移植过来以后参考了.NET程序集的组织结构，
如果你从这个角度去看log4net，你就会发现其设计的精妙之处，你也可以遵循这个思路设计出自己的日志管理系统：    

![image](https://github.com/bzmework/log4net/blob/master/log4net.jpg)     

坦率的说，log4net的输出样式支持的各种参数有些我是不满意的，但不想花太多精力在上面，于是借助其GlobalContext换了一种精简的日志输出方式(仅供参考)：   
```
        // 获得一个日志器
        private static readonly ILog log = LogManager.GetLogger(typeof(Program)); 

        // 基本的配置
        BasicConfigurator.Configure(true, true);

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="e"></param>
        private static void WriteLog(Exception e, string methodName= "", string className= "", string extraData = "")
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
 ```

log4net的配置非常灵活，详细请参见示例自己调试，下图是输出效果:      

![image](https://github.com/bzmework/log4net/blob/master/test.jpg)     
