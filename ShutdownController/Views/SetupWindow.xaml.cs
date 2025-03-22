using System;
using System.Windows;
using ShutdownController.ViewModels;

namespace ShutdownController.Views
{
    /// <summary>
    /// SetupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetupWindow : Window
    {
        private readonly SetupViewModel _viewModel;
        
        /// <summary>
        /// 是否成功完成设置
        /// </summary>
        public bool IsSetupCompleted { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public SetupWindow()
        {
            InitializeComponent();
            
            _viewModel = new SetupViewModel();
            DataContext = _viewModel;
            
            // 注册事件
            _viewModel.SetupCompleted += ViewModel_SetupCompleted;
            
            // 窗口关闭时取消注册事件
            Closed += (s, e) => {
                _viewModel.SetupCompleted -= ViewModel_SetupCompleted;
            };
            
            // 设置初始焦点
            Loaded += (s, e) => PasswordBox.Focus();
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
        /// 确认密码框密码变更事件处理
        /// </summary>
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }
        
        /// <summary>
        /// 设置完成事件处理
        /// </summary>
        private void ViewModel_SetupCompleted(object sender, bool isCompleted)
        {
            IsSetupCompleted = isCompleted;
            Close();
        }
    }
} 