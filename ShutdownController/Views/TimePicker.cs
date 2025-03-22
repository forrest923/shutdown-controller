using System;
using System.Windows;
using System.Windows.Controls;

namespace ShutdownController.Views
{
    /// <summary>
    /// 时间选择器控件
    /// </summary>
    public class TimePicker : Control
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }
        
        /// <summary>
        /// 小时依赖属性
        /// </summary>
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(int), typeof(TimePicker), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceHour));
                
        /// <summary>
        /// 分钟依赖属性
        /// </summary>
        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register("Minute", typeof(int), typeof(TimePicker), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceMinute));
                
        /// <summary>
        /// 选中时间依赖属性
        /// </summary>
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(TimeSpan), typeof(TimePicker), 
                new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged));
        
        /// <summary>
        /// 小时
        /// </summary>
        public int Hour
        {
            get { return (int)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }
        
        /// <summary>
        /// 分钟
        /// </summary>
        public int Minute
        {
            get { return (int)GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }
        
        /// <summary>
        /// 选中时间
        /// </summary>
        public TimeSpan SelectedTime
        {
            get { return (TimeSpan)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }
        
        /// <summary>
        /// 选中时间变更时更新小时和分钟
        /// </summary>
        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            TimeSpan time = (TimeSpan)e.NewValue;
            
            timePicker.SetCurrentValue(HourProperty, time.Hours);
            timePicker.SetCurrentValue(MinuteProperty, time.Minutes);
        }
        
        /// <summary>
        /// 小时或分钟变更时更新选中时间
        /// </summary>
        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            TimeSpan newTime = new TimeSpan(timePicker.Hour, timePicker.Minute, 0);
            
            timePicker.SetCurrentValue(SelectedTimeProperty, newTime);
        }
        
        /// <summary>
        /// 确保小时值在合法范围内
        /// </summary>
        private static object CoerceHour(DependencyObject d, object baseValue)
        {
            int value = (int)baseValue;
            if (value < 0) return 0;
            if (value > 23) return 23;
            return value;
        }
        
        /// <summary>
        /// 确保分钟值在合法范围内
        /// </summary>
        private static object CoerceMinute(DependencyObject d, object baseValue)
        {
            int value = (int)baseValue;
            if (value < 0) return 0;
            if (value > 59) return 59;
            return value;
        }
    }
} 