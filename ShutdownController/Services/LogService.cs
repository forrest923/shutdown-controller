using System;
using System.IO;
using System.Text;
using ShutdownController.Models;

namespace ShutdownController.Services
{
    /// <summary>
    /// 日志服务类
    /// </summary>
    public static class LogService
    {
        private static readonly object _lock = new object();
        
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogInfo(string message)
        {
            Log(new LogEntry(LogType.Info, message));
        }
        
        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogWarning(string message)
        {
            Log(new LogEntry(LogType.Warning, message));
        }
        
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="exception">相关异常</param>
        public static void LogError(string message, Exception exception = null)
        {
            Log(new LogEntry(LogType.Error, message, exception));
        }
        
        /// <summary>
        /// 记录关机日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogShutdown(string message)
        {
            Log(new LogEntry(LogType.Shutdown, message));
        }
        
        /// <summary>
        /// 记录日志条目
        /// </summary>
        /// <param name="entry">日志条目</param>
        private static void Log(LogEntry entry)
        {
            try
            {
                string logFilePath = GetLogFilePath(entry.Timestamp);
                
                // 确保日志目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                
                StringBuilder sb = new StringBuilder();
                sb.Append($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] ");
                sb.Append($"[{entry.Type}] ");
                sb.Append(entry.Message);
                
                if (!string.IsNullOrEmpty(entry.Exception))
                {
                    sb.AppendLine();
                    sb.Append(entry.Exception);
                }
                
                lock (_lock)
                {
                    // 以追加模式写入文件
                    using (StreamWriter writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
                    {
                        writer.WriteLine(sb.ToString());
                    }
                }
            }
            catch
            {
                // 日志记录失败时，避免影响应用程序正常运行
            }
        }
        
        /// <summary>
        /// 获取日志文件路径
        /// </summary>
        /// <param name="timestamp">日志时间戳</param>
        /// <returns>日志文件路径</returns>
        private static string GetLogFilePath(DateTime timestamp)
        {
            string logFileName = $"log_{timestamp:yyyy-MM-dd}.txt";
            return Path.Combine(ConfigService.GetLogPath(), logFileName);
        }
    }
} 