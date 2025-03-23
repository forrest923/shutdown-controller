using System;
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
                InitializeComponent();
                
                _viewModel = new MainViewModel();
                DataContext = _viewModel;
                
                // 注册事件
                _viewModel.WindowClose += ViewModel_WindowClose;
                
                // 窗口关闭时取消注册事件
                Closed += (s, e) => {
                    _viewModel.WindowClose -= ViewModel_WindowClose;
                };
                
                LogService.LogInfo("主窗口成功初始化");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化界面时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogService.LogError("初始化主窗口失败", ex);
                Close();
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
                LogService.LogError("关闭主窗口时出错", ex);
                Application.Current.Shutdown();
            }
        }
    }
} 