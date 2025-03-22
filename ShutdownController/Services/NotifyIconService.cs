using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ShutdownController.Views;

namespace ShutdownController.Services
{
    /// <summary>
    /// 托盘图标服务类
    /// </summary>
    public class NotifyIconService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private bool _disposed = false;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyIconService()
        {
            // 创建托盘图标
            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location),
                Text = "关机控制器",
                Visible = true
            };
            
            // 创建右键菜单
            ContextMenu contextMenu = new ContextMenu();
            MenuItem settingsMenuItem = new MenuItem("设置", OnSettingsClick);
            MenuItem exitMenuItem = new MenuItem("退出", OnExitClick);
            
            contextMenu.MenuItems.Add(settingsMenuItem);
            contextMenu.MenuItems.Add(new MenuItem("-")); // 分隔线
            contextMenu.MenuItems.Add(exitMenuItem);
            
            _notifyIcon.ContextMenu = contextMenu;
            
            // 双击事件
            _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;
        }
        
        /// <summary>
        /// 托盘图标双击事件处理
        /// </summary>
        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            ShowLoginWindow();
        }
        
        /// <summary>
        /// 设置菜单项点击事件处理
        /// </summary>
        private void OnSettingsClick(object sender, EventArgs e)
        {
            ShowLoginWindow();
        }
        
        /// <summary>
        /// 退出菜单项点击事件处理
        /// </summary>
        private void OnExitClick(object sender, EventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow(isExitMode: true);
            loginWindow.ShowDialog();
        }
        
        /// <summary>
        /// 显示登录窗口
        /// </summary>
        private void ShowLoginWindow()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                }
                
                _disposed = true;
            }
        }
    }
} 