using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShutdownController.ViewModels
{
    /// <summary>
    /// ViewModel基类，实现INotifyPropertyChanged接口
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 触发属性变更事件
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// 设置属性值并触发属性变更事件
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="storage">属性存储引用</param>
        /// <param name="value">新属性值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>如果值已更改，则返回true</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
                
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
} 