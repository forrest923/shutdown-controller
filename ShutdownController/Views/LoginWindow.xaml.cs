using System;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化登录窗口失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogService.LogError("初始化登录窗口失败", ex);
                Close();
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
                if (!_viewModel.IsExitMode)
                {
                    try
                    {
                        // 打开主设置窗口
                        var mainWindow = new MainWindow();
                        mainWindow.ShowDialog(); // 使用ShowDialog代替Show，确保主窗口显示
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"打开设置窗口失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        LogService.LogError("打开设置窗口失败", ex);
                    }
                }
                
                Close();
            }
            catch (Exception ex)
            {
                LogService.LogError("登录成功事件处理失败", ex);
            }
        }
        
        /// <summary>
        /// 应用程序退出事件处理
        /// </summary>
        private void ViewModel_AppExit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
} 