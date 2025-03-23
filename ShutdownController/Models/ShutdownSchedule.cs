using System;
using System.Collections.Generic;
using ShutdownController.ViewModels;

namespace ShutdownController.Models
{
    /// <summary>
    /// 关机计划模式枚举
    /// </summary>
    public enum ScheduleMode
    {
        /// <summary>
        /// 按星期几模式
        /// </summary>
        WeekDay,
        
        /// <summary>
        /// 按日期范围模式
        /// </summary>
        DateRange
    }

    /// <summary>
    /// 关机计划类
    /// </summary>
    [Serializable]
    public class ShutdownSchedule
    {
        /// <summary>
        /// 计划ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 计划模式
        /// </summary>
        public ScheduleMode Mode { get; set; }
        
        /// <summary>
        /// 计划名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// 按日期范围的开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// 按日期范围的结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// 按星期几的关机计划
        /// </summary>
        [NonSerialized]
        private Dictionary<DayOfWeek, TimeSpan> _weekDayShutdownTimes;
        
        /// <summary>
        /// 按星期几的关机计划
        /// </summary>
        public Dictionary<DayOfWeek, TimeSpan> WeekDayShutdownTimes 
        {
            get => _weekDayShutdownTimes;
            set => _weekDayShutdownTimes = value;
        }
        
        /// <summary>
        /// 按日期范围的关机时间
        /// </summary>
        public TimeSpan? DateRangeShutdownTime { get; set; }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShutdownSchedule()
        {
            Id = Guid.NewGuid();
            Mode = ScheduleMode.WeekDay;
            IsEnabled = true;
            _weekDayShutdownTimes = new Dictionary<DayOfWeek, TimeSpan>();
        }
        
        /// <summary>
        /// 检查当前时间是否符合关机条件
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        /// <returns>如果满足关机条件返回true，否则返回false</returns>
        public bool ShouldShutdown(DateTime currentTime)
        {
            if (!IsEnabled)
                return false;
                
            if (Mode == ScheduleMode.DateRange)
            {
                // 按日期范围检查
                if (StartDate.HasValue && EndDate.HasValue && DateRangeShutdownTime.HasValue)
                {
                    DateTime currentDate = currentTime.Date;
                    TimeSpan currentTimeOfDay = currentTime.TimeOfDay;
                    
                    if (currentDate >= StartDate.Value.Date && currentDate <= EndDate.Value.Date)
                    {
                        return currentTimeOfDay >= DateRangeShutdownTime.Value;
                    }
                }
            }
            else // WeekDay模式
            {
                // 按星期几检查
                if (WeekDayShutdownTimes.TryGetValue(currentTime.DayOfWeek, out TimeSpan shutdownTime))
                {
                    return currentTime.TimeOfDay >= shutdownTime;
                }
            }
            
            return false;
        }
    }
} 