using System;
using System.Windows;
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
            InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            // 注册事件
            _viewModel.WindowClose += ViewModel_WindowClose;
            
            // 窗口关闭时取消注册事件
            Closed += (s, e) => {
                _viewModel.WindowClose -= ViewModel_WindowClose;
            };
        }
        
        /// <summary>
        /// 窗口关闭事件处理
        /// </summary>
        private void ViewModel_WindowClose(object sender, EventArgs e)
        {
            Close();
        }
    }
} 