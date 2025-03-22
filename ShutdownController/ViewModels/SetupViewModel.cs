using System;
using System.Windows;
using System.Windows.Input;
using ShutdownController.Services;

namespace ShutdownController.ViewModels
{
    /// <summary>
    /// 初始设置视图模型
    /// </summary>
    public class SetupViewModel : ViewModelBase
    {
        private string _password;
        private string _confirmPassword;
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; }
        
        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand { get; }
        
        /// <summary>
        /// 设置完成事件
        /// </summary>
        public event EventHandler<bool> SetupCompleted;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public SetupViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }
        
        /// <summary>
        /// 判断是否可以保存
        /// </summary>
        /// <returns>如果可以保存返回true，否则返回false</returns>
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Password) && 
                !string.IsNullOrWhiteSpace(ConfirmPassword);
        }
        
        /// <summary>
        /// 执行保存操作
        /// </summary>
        private void Save()
        {
            try
            {
                if (Password != ConfirmPassword)
                {
                    MessageBox.Show("两次输入的密码不一致，请重新输入！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;
                    return;
                }
                
                // 设置密码
                ConfigService.SetPassword(Password);
                
                // 触发设置完成事件
                SetupCompleted?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置密码失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogService.LogError("设置密码失败", ex);
            }
        }
        
        /// <summary>
        /// 取消操作
        /// </summary>
        private void Cancel()
        {
            // 触发设置取消事件
            SetupCompleted?.Invoke(this, false);
        }
    }
} 