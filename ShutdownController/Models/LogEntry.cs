using System;

namespace ShutdownController.Models
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        
        /// <summary>
        /// 关机
        /// </summary>
        Shutdown
    }

    /// <summary>
    /// 日志条目类
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType Type { get; set; }
        
        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// 相关异常信息（如有）
        /// </summary>
        public string Exception { get; set; }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LogEntry()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }
        
        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常（可选）</param>
        public LogEntry(LogType type, string message, Exception exception = null)
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
            Type = type;
            Message = message;
            Exception = exception?.ToString();
        }
    }
} 