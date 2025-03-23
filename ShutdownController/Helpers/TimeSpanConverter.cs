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

        /// <summary>
        /// 将TimeWrapper转换为TimeSpan
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeWrapper timeWrapper)
            {
                return timeWrapper.TimeSpan;
            }
            
            return TimeSpan.Zero;
        }
    }
} 