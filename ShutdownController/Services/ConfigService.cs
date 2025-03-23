using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using ShutdownController.Models;

namespace ShutdownController.Services
{
    /// <summary>
    /// 配置服务类
    /// </summary>
    public static class ConfigService
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ShutdownController",
            "config.xml");
            
        private static AppConfig _config;
        private static readonly object _lock = new object();
        private const string FixedPassword = "12345678a";
        
        /// <summary>
        /// 获取应用配置
        /// </summary>
        /// <returns>应用配置对象</returns>
        public static AppConfig GetConfig()
        {
            if (_config != null)
                return _config;
                
            lock (_lock)
            {
                if (_config != null)
                    return _config;
                    
                _config = LoadConfig();
                return _config;
            }
        }
        
        /// <summary>
        /// 保存应用配置
        /// </summary>
        /// <param name="config">要保存的配置对象</param>
        public static void SaveConfig(AppConfig config)
        {
            lock (_lock)
            {
                _config = config;
                SaveConfigToFile(config);
            }
        }
        
        /// <summary>
        /// 检查是否首次运行
        /// </summary>
        /// <returns>如果是首次运行返回true，否则返回false</returns>
        public static bool IsFirstRun()
        {
            return GetConfig().IsFirstRun;
        }
        
        /// <summary>
        /// 设置固定密码
        /// </summary>
        public static void SetFixedPassword()
        {
            var config = GetConfig();
            config.PasswordHash = HashPassword(FixedPassword);
            config.IsFirstRun = false;
            SaveConfig(config);
            LogService.LogInfo("已设置固定密码");
        }
        
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="password">密码明文</param>
        public static void SetPassword(string password)
        {
            var config = GetConfig();
            config.PasswordHash = HashPassword(password);
            config.IsFirstRun = false;
            SaveConfig(config);
        }
        
        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">要验证的密码明文</param>
        /// <returns>如果密码正确返回true，否则返回false</returns>
        public static bool VerifyPassword(string password)
        {
            var config = GetConfig();
            string hashToCheck = HashPassword(password);
            return hashToCheck == config.PasswordHash;
        }
        
        /// <summary>
        /// 获取日志路径
        /// </summary>
        /// <returns>日志目录路径</returns>
        public static string GetLogPath()
        {
            string configuredPath = ConfigurationManager.AppSettings["LogPath"];
            
            // 如果是相对路径，转为绝对路径
            if (!Path.IsPathRooted(configuredPath))
            {
                configuredPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ShutdownController",
                    configuredPath);
            }
            
            return configuredPath;
        }
        
        /// <summary>
        /// 对密码进行哈希处理
        /// </summary>
        /// <param name="password">要哈希的密码明文</param>
        /// <returns>哈希值字符串</returns>
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }
        
        /// <summary>
        /// 从文件加载配置
        /// </summary>
        /// <returns>加载的配置对象</returns>
        private static AppConfig LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                // 确保配置目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
                return new AppConfig();
            }
            
            try
            {
                using (FileStream stream = new FileStream(ConfigFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                    return (AppConfig)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("加载配置文件失败", ex);
                return new AppConfig();
            }
        }
        
        /// <summary>
        /// 保存配置到文件
        /// </summary>
        /// <param name="config">要保存的配置对象</param>
        private static void SaveConfigToFile(AppConfig config)
        {
            try
            {
                // 确保配置目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
                
                using (FileStream stream = new FileStream(ConfigFilePath, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                    serializer.Serialize(stream, config);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("保存配置文件失败", ex);
            }
        }
    }
} 