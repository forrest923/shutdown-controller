using System;
using System.Windows;
using System.Windows.Input;
using ShutdownController.Services;

namespace ShutdownController.ViewModels
{
    /// <summary>
    /// 登录视图模型
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private string _password;
        private bool _isExitMode;
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        /// <summary>
        /// 是否是退出模式
        /// </summary>
        public bool IsExitMode
        {
            get => _isExitMode;
            set => SetProperty(ref _isExitMode, value);
        }
        
        /// <summary>
        /// 登录命令
        /// </summary>
        public ICommand LoginCommand { get; }
        
        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand { get; }
        
        /// <summary>
        /// 登录成功事件
        /// </summary>
        public event EventHandler LoginSuccessful;
        
        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        public event EventHandler AppExit;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isExitMode">是否是退出模式</param>
        public LoginViewModel(bool isExitMode = false)
        {
            _isExitMode = isExitMode;
            
            LoginCommand = new RelayCommand(Login, CanLogin);
            CancelCommand = new RelayCommand(Cancel);
        }
        
        /// <summary>
        /// 判断是否可以登录
        /// </summary>
        /// <returns>如果可以登录返回true，否则返回false</returns>
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Password);
        }
        
        /// <summary>
        /// 执行登录操作
        /// </summary>
        private void Login()
        {
            try
            {
                if (ConfigService.VerifyPassword(Password))
                {
                    if (IsExitMode)
                    {
                        // 退出应用程序
                        AppExit?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        // 登录成功
                        LoginSuccessful?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    MessageBox.Show("密码错误，请重新输入！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"登录失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogService.LogError("登录失败", ex);
            }
        }
        
        /// <summary>
        /// 取消操作
        /// </summary>
        private void Cancel()
        {
            // 触发窗口关闭
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }
    }
    
    /// <summary>
    /// 命令实现类
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        
        /// <summary>
        /// 可执行性变化事件
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execute">执行动作</param>
        /// <param name="canExecute">可执行性判断函数</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        /// <summary>
        /// 判断命令是否可执行
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>如果可执行返回true，否则返回false</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        public void Execute(object parameter)
        {
            _execute();
        }
    }
} 