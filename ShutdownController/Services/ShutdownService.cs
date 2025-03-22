using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using ShutdownController.Models;

namespace ShutdownController.Services
{
    /// <summary>
    /// 关机服务类
    /// </summary>
    public class ShutdownService
    {
        private Timer _checkTimer;
        private Timer _notifyTimer;
        private DateTime _scheduledShutdownTime;
        private bool _isShutdownScheduled;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // 每分钟检查一次
        
        /// <summary>
        /// 启动关机服务
        /// </summary>
        public void Start()
        {
            LogService.LogInfo("关机服务已启动");
            
            // 停止旧的计时器
            StopTimers();
            
            // 创建新的关机检查计时器
            _checkTimer = new Timer(CheckShutdownSchedule, null, TimeSpan.Zero, _checkInterval);
        }
        
        /// <summary>
        /// 停止关机服务
        /// </summary>
        public void Stop()
        {
            LogService.LogInfo("关机服务已停止");
            
            // 停止所有计时器
            StopTimers();
        }
        
        /// <summary>
        /// 检查关机计划
        /// </summary>
        private void CheckShutdownSchedule(object state)
        {
            try
            {
                DateTime now = DateTime.Now;
                var config = ConfigService.GetConfig();
                
                foreach (var schedule in config.Schedules)
                {
                    if (schedule.IsEnabled && schedule.ShouldShutdown(now))
                    {
                        // 找到第一个应当关机的计划
                        if (!_isShutdownScheduled)
                        {
                            // 计算实际关机时间（当前时间+10分钟）
                            _scheduledShutdownTime = now.AddMinutes(10);
                            _isShutdownScheduled = true;
                            
                            // 显示通知
                            ShowShutdownNotification();
                            
                            // 安排10分钟后关机
                            _notifyTimer = new Timer(ExecuteShutdown, null, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
                            
                            LogService.LogInfo($"计划关机已安排在 {_scheduledShutdownTime:yyyy-MM-dd HH:mm:ss}");
                        }
                        
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("检查关机计划时出错", ex);
            }
        }
        
        /// <summary>
        /// 显示关机通知
        /// </summary>
        private void ShowShutdownNotification()
        {
            try
            {
                // 在UI线程上执行
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        "同学，为了您的健康，系统将在10分钟后关机",
                        "系统关机通知",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                });
            }
            catch (Exception ex)
            {
                LogService.LogError("显示关机通知时出错", ex);
            }
        }
        
        /// <summary>
        /// 执行系统关机
        /// </summary>
        private void ExecuteShutdown(object state)
        {
            try
            {
                LogService.LogShutdown($"系统执行关机操作 ({DateTime.Now:yyyy-MM-dd HH:mm:ss})");
                
                // 重置状态
                _isShutdownScheduled = false;
                
                // 执行关机命令
                Process.Start("shutdown", "/s /f /t 0");
            }
            catch (Exception ex)
            {
                LogService.LogError("执行系统关机时出错", ex);
            }
        }
        
        /// <summary>
        /// 停止所有计时器
        /// </summary>
        private void StopTimers()
        {
            _checkTimer?.Dispose();
            _notifyTimer?.Dispose();
            _checkTimer = null;
            _notifyTimer = null;
            _isShutdownScheduled = false;
        }
    }
} 