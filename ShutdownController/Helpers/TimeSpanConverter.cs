using System;
using System.Globalization;
using System.Windows.Data;
using ShutdownController.ViewModels;

namespace ShutdownController.Helpers
{
    /// <summary>
    /// TimeSpan和TimeWrapper之间的转换器
    /// </summary>
    public class TimeSpanConverter : IValueConverter
    {
        /// <summary>
        /// 将TimeSpan转换为TimeWrapper
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                LogService.LogInfo($"TimeSpanConverter.Convert - 输入值类型: {value?.GetType().Name}, 目标类型: {targetType.Name}");
                
                if (value is TimeSpan timeSpan)
                {
                    return new TimeWrapper(timeSpan);
                }
                
                if (value is TimeSpan? nullableTimeSpan && nullableTimeSpan.HasValue)
                {
                    return new TimeWrapper(nullableTimeSpan.Value);
                }
                
                return new TimeWrapper(new TimeSpan(0, 0, 0));
            }
            catch (Exception ex)
            {
                LogService.LogError("TimeSpanConverter.Convert 转换失败", ex);
                return new TimeWrapper(new TimeSpan(0, 0, 0));
            }
        }

        /// <summary>
        /// 将TimeWrapper转换为TimeSpan
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                LogService.LogInfo($"TimeSpanConverter.ConvertBack - 输入值类型: {value?.GetType().Name}, 目标类型: {targetType.Name}");
                
                if (value is TimeWrapper timeWrapper)
                {
                    return timeWrapper.TimeSpan;
                }
                
                if (value is string timeString && int.TryParse(timeString, out int timeValue))
                {
                    // 根据参数决定是小时还是分钟
                    if (parameter != null && parameter.ToString() == "Minutes")
                    {
                        return new TimeSpan(0, timeValue, 0);
                    }
                    else
                    {
                        return new TimeSpan(timeValue, 0, 0);
                    }
                }
                
                return TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                LogService.LogError("TimeSpanConverter.ConvertBack 转换失败", ex);
                return TimeSpan.Zero;
            }
        }
    }
} 