using System;
using System.Collections.Generic;

namespace ShutdownController.Models
{
    /// <summary>
    /// 应用程序配置类
    /// </summary>
    [Serializable]
    public class AppConfig
    {
        /// <summary>
        /// 密码的哈希值
        /// </summary>
        public string PasswordHash { get; set; }
        
        /// <summary>
        /// 关机计划列表
        /// </summary>
        public List<ShutdownSchedule> Schedules { get; set; }
        
        /// <summary>
        /// 是否首次运行
        /// </summary>
        public bool IsFirstRun { get; set; }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AppConfig()
        {
            IsFirstRun = true;
            Schedules = new List<ShutdownSchedule>();
        }
    }
} 