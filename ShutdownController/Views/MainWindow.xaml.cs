using System;
using System.Diagnostics;
using System.Windows;
using ShutdownController.Services;
using ShutdownController.ViewModels;

namespace ShutdownController.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            try
            {
                LogService.LogInfo("开始初始化MainWindow");
                
                // 初始化组件
                InitializeComponent();
                
                // 创建并设置ViewModel
                try
                {
                    _viewModel = new MainViewModel();
                    DataContext = _viewModel;
                    
                    LogService.LogInfo("MainViewModel创建成功");
                }
                catch (Exception ex)
                {
                    LogService.LogError("创建MainViewModel失败", ex);
                    MessageBox.Show($"创建视图模型失败: {ex.Message}\n\n详细信息: {ex.StackTrace}", 
                        "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }
                
                // 注册事件
                _viewModel.WindowClose += ViewModel_WindowClose;
                
                // 窗口关闭时取消注册事件
                Closed += (s, e) => {
                    _viewModel.WindowClose -= ViewModel_WindowClose;
                };
                
                LogService.LogInfo("MainWindow初始化完成");
            }
            catch (Exception ex)
            {
                LogService.LogError("初始化MainWindow失败", ex);
                MessageBox.Show($"初始化界面时出错：{ex.Message}\n\n详细信息: {ex.StackTrace}", 
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
        /// 窗口关闭事件处理
        /// </summary>
        private void ViewModel_WindowClose(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                LogService.LogError("关闭MainWindow时出错", ex);
                try
                {
                    Application.Current.Shutdown();
                }
                catch
                {
                    // 忽略强制关闭时的异常
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
    }
} 