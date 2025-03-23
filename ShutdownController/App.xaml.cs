using System;
using System.Windows;
using System.IO;
using System.Threading;
using ShutdownController.Services;
using ShutdownController.Views;
using Microsoft.Win32;

namespace ShutdownController
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex _appMutex;
        private ShutdownService _shutdownService;
        private NotifyIconService _notifyIconService;
        private const string AppName = "ShutdownController";
        private const string MutexName = "Global\\ShutdownControllerMutex";

        protected override void OnStartup(StartupEventArgs e)
        {
            // 确保应用程序只有一个实例运行
            bool createdNew;
            _appMutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("应用程序已经在运行中！", AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // 初始化日志目录
            InitializeLogDirectory();

            // 添加到开机启动
            SetStartup();

            // 创建服务
            _shutdownService = new ShutdownService();
            _notifyIconService = new NotifyIconService();

            // 检查是否首次运行
            if (ConfigService.IsFirstRun())
            {
                ShowSetupWindow();
            }
            else
            {
                // 启动关机服务
                _shutdownService.Start();
                
                // 如果有命令行参数"-setup"，显示设置窗口
                if (e.Args.Length > 0 && e.Args[0].ToLower() == "-setup")
                {
                    ShowLoginWindow();
                }
            }
        }

        /// <summary>
        /// 显示首次运行的设置窗口
        /// </summary>
        private void ShowSetupWindow()
        {
            var setupWindow = new SetupWindow();
            setupWindow.ShowDialog();
            
            if (setupWindow.IsSetupCompleted)
            {
                _shutdownService.Start();
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// 显示登录窗口
        /// </summary>
        private void ShowLoginWindow()
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        /// <summary>
        /// 初始化日志目录
        /// </summary>
        private void InitializeLogDirectory()
        {
            string logPath = ConfigService.GetLogPath();
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        /// <summary>
        /// 设置开机启动
        /// </summary>
        private void SetStartup()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    // 获取应用程序路径
                    string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    key.SetValue(AppName, $"\"{appPath}\"");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("设置开机启动失败", ex);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 停止并清理服务
            _shutdownService?.Stop();
            _notifyIconService?.Dispose();
            _appMutex?.ReleaseMutex();
            
            base.OnExit(e);
        }
    }
} 