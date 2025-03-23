using System;
using System.Diagnostics;
using System.Windows;
using ShutdownController.Services;
using ShutdownController.ViewModels;

namespace ShutdownController.Views
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isExitMode">是否是退出模式</param>
        public LoginWindow(bool isExitMode = false)
        {
            try
            {
                LogService.LogInfo("开始初始化LoginWindow");
                
                InitializeComponent();
                
                _viewModel = new LoginViewModel(isExitMode);
                DataContext = _viewModel;
                
                // 注册事件
                _viewModel.LoginSuccessful += ViewModel_LoginSuccessful;
                _viewModel.AppExit += ViewModel_AppExit;
                
                // 窗口关闭时取消注册事件
                Closed += (s, e) => {
                    _viewModel.LoginSuccessful -= ViewModel_LoginSuccessful;
                    _viewModel.AppExit -= ViewModel_AppExit;
                };
                
                // 设置初始焦点
                Loaded += (s, e) => PasswordBox.Focus();
                
                LogService.LogInfo("LoginWindow初始化完成");
            }
            catch (Exception ex)
            {
                LogService.LogError("初始化登录窗口失败", ex);
                MessageBox.Show($"初始化登录窗口失败：{ex.Message}\n\n详细信息：{ex.StackTrace}", 
                    "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                try
                {
                    Close();
                }
                catch
                {
                    // 忽略关闭时的异常
                }
            }
        }
        
        /// <summary>
        /// 密码框密码变更事件处理
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Password = PasswordBox.Password;
            }
        }
        
        /// <summary>
        /// 登录成功事件处理
        /// </summary>
        private void ViewModel_LoginSuccessful(object sender, EventArgs e)
        {
            try
            {
                LogService.LogInfo("登录验证成功");
                
                if (!_viewModel.IsExitMode)
                {
                    try
                    {
                        LogService.LogInfo("开始打开MainWindow");
                        
                        // 先关闭登录窗口
                        this.Hide();
                        
                        // 打开主设置窗口
                        var mainWindow = new MainWindow();
                        
                        // 确保主窗口关闭时不会影响程序继续运行
                        mainWindow.Closed += (s, args) => {
                            LogService.LogInfo("MainWindow已关闭");
                            this.Close();
                        };
                        
                        // 显示主窗口
                        mainWindow.Show();
                        
                        // 不立即关闭登录窗口，等主窗口关闭时再关闭
                        return;
                    }
                    catch (Exception ex)
                    {
                        LogService.LogError("打开设置窗口失败", ex);
                        MessageBox.Show($"打开设置窗口失败：{ex.Message}\n\n详细信息：{ex.StackTrace}", 
                            "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        
                        // 显示失败后再显示登录窗口
                        this.Show();
                    }
                }
                
                // 如果是退出模式或者显示主窗口失败，则关闭登录窗口
                Close();
            }
            catch (Exception ex)
            {
                LogService.LogError("登录成功事件处理失败", ex);
                try
                {
                    Close();
                }
                catch
                {
                    // 忽略关闭时的异常
                }
            }
        }
        
        /// <summary>
        /// 应用程序退出事件处理
        /// </summary>
        private void ViewModel_AppExit(object sender, EventArgs e)
        {
            try
            {
                LogService.LogInfo("应用程序准备退出");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                LogService.LogError("应用程序退出失败", ex);
                try
                {
                    Process.GetCurrentProcess().Kill();
                }
                catch
                {
                    // 忽略强制退出时的异常
                }
            }
        }
    }
} 