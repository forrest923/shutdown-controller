using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ShutdownController.Models;
using ShutdownController.Services;

namespace ShutdownController.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ShutdownSchedule _selectedSchedule;
        private ScheduleMode _selectedMode;
        private bool _isDateRangeMode;
        private bool _isWeekDayMode;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private TimeWrapper _dateRangeTime;
        private ObservableCollection<DayTimeViewModel> _weekDayTimes;
        
        /// <summary>
        /// 关机计划列表
        /// </summary>
        public ObservableCollection<ShutdownSchedule> Schedules { get; }
        
        /// <summary>
        /// 当前选中的关机计划
        /// </summary>
        public ShutdownSchedule SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                if (SetProperty(ref _selectedSchedule, value))
                {
                    LoadScheduleDetails();
                }
            }
        }
        
        /// <summary>
        /// 当前选择的计划模式
        /// </summary>
        public ScheduleMode SelectedMode
        {
            get => _selectedMode;
            set
            {
                if (SetProperty(ref _selectedMode, value))
                {
                    IsDateRangeMode = (value == ScheduleMode.DateRange);
                    IsWeekDayMode = (value == ScheduleMode.WeekDay);
                }
            }
        }
        
        /// <summary>
        /// 是否是日期范围模式
        /// </summary>
        public bool IsDateRangeMode
        {
            get => _isDateRangeMode;
            set => SetProperty(ref _isDateRangeMode, value);
        }
        
        /// <summary>
        /// 是否是按星期几模式
        /// </summary>
        public bool IsWeekDayMode
        {
            get => _isWeekDayMode;
            set => SetProperty(ref _isWeekDayMode, value);
        }
        
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }
        
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }
        
        /// <summary>
        /// 日期范围关机时间
        /// </summary>
        public TimeWrapper DateRangeTime
        {
            get => _dateRangeTime;
            set => SetProperty(ref _dateRangeTime, value);
        }
        
        /// <summary>
        /// 星期几关机时间集合
        /// </summary>
        public ObservableCollection<DayTimeViewModel> WeekDayTimes
        {
            get => _weekDayTimes;
            set => SetProperty(ref _weekDayTimes, value);
        }
        
        /// <summary>
        /// 新增计划命令
        /// </summary>
        public ICommand AddScheduleCommand { get; }
        
        /// <summary>
        /// 删除计划命令
        /// </summary>
        public ICommand DeleteScheduleCommand { get; }
        
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; }
        
        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand { get; }
        
        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event EventHandler WindowClose;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainViewModel()
        {
            // 初始化集合
            Schedules = new ObservableCollection<ShutdownSchedule>();
            WeekDayTimes = new ObservableCollection<DayTimeViewModel>();
            
            // 初始化命令
            AddScheduleCommand = new RelayCommand(AddSchedule);
            DeleteScheduleCommand = new RelayCommand(DeleteSchedule, CanDeleteSchedule);
            SaveCommand = new RelayCommand(Save, CanSave);
            CloseCommand = new RelayCommand(Close);
            
            // 加载配置
            LoadConfig();
            
            // 默认按星期几模式
            SelectedMode = ScheduleMode.WeekDay;
            
            // 设置默认时间
            DateRangeTime = new TimeWrapper(new TimeSpan(21, 0, 0)); // 默认晚上9点
        }
        
        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfig()
        {
            var config = ConfigService.GetConfig();
            
            Schedules.Clear();
            foreach (var schedule in config.Schedules)
            {
                Schedules.Add(schedule);
            }
            
            if (Schedules.Count > 0)
            {
                SelectedSchedule = Schedules[0];
            }
            else
            {
                InitializeWeekDayTimes();
            }
        }
        
        /// <summary>
        /// 初始化星期几视图模型
        /// </summary>
        private void InitializeWeekDayTimes()
        {
            WeekDayTimes.Clear();
            
            // 添加周一到周日
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Monday, "周一"));
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Tuesday, "周二"));
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Wednesday, "周三"));
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Thursday, "周四"));
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Friday, "周五"));
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Saturday, "周六"));
            WeekDayTimes.Add(new DayTimeViewModel(DayOfWeek.Sunday, "周日"));
            
            // 设置默认时间为晚上9点
            foreach (var dayTime in WeekDayTimes)
            {
                dayTime.Time = new TimeWrapper(new TimeSpan(21, 0, 0));
                dayTime.IsEnabled = true;
            }
        }
        
        /// <summary>
        /// 加载选中计划的详细信息
        /// </summary>
        private void LoadScheduleDetails()
        {
            if (SelectedSchedule == null)
                return;
                
            // 设置模式
            SelectedMode = SelectedSchedule.Mode;
            
            if (SelectedSchedule.Mode == ScheduleMode.DateRange)
            {
                // 加载日期范围模式的数据
                StartDate = SelectedSchedule.StartDate;
                EndDate = SelectedSchedule.EndDate;
                DateRangeTime = new TimeWrapper(SelectedSchedule.DateRangeShutdownTime ?? new TimeSpan(21, 0, 0));
            }
            else
            {
                // 加载星期几模式的数据
                InitializeWeekDayTimes();
                
                foreach (var dayTime in WeekDayTimes)
                {
                    if (SelectedSchedule.WeekDayShutdownTimes.TryGetValue(dayTime.DayOfWeek, out TimeSpan time))
                    {
                        dayTime.Time = new TimeWrapper(time);
                        dayTime.IsEnabled = true;
                    }
                    else
                    {
                        dayTime.IsEnabled = false;
                    }
                }
            }
        }
        
        /// <summary>
        /// 添加新计划
        /// </summary>
        private void AddSchedule()
        {
            var newSchedule = new ShutdownSchedule
            {
                Name = $"计划 {Schedules.Count + 1}",
                Mode = ScheduleMode.WeekDay
            };
            
            Schedules.Add(newSchedule);
            SelectedSchedule = newSchedule;
        }
        
        /// <summary>
        /// 判断是否可以删除计划
        /// </summary>
        /// <returns>如果可以删除返回true，否则返回false</returns>
        private bool CanDeleteSchedule()
        {
            return SelectedSchedule != null;
        }
        
        /// <summary>
        /// 删除选中的计划
        /// </summary>
        private void DeleteSchedule()
        {
            if (SelectedSchedule == null)
                return;
                
            if (MessageBox.Show("确定要删除所选计划吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Schedules.Remove(SelectedSchedule);
                
                if (Schedules.Count > 0)
                {
                    SelectedSchedule = Schedules[0];
                }
                else
                {
                    SelectedSchedule = null;
                    InitializeWeekDayTimes();
                }
            }
        }
        
        /// <summary>
        /// 判断是否可以保存
        /// </summary>
        /// <returns>如果可以保存返回true，否则返回false</returns>
        private bool CanSave()
        {
            if (SelectedSchedule == null)
                return false;
                
            if (SelectedMode == ScheduleMode.DateRange)
            {
                return StartDate.HasValue && EndDate.HasValue;
            }
            
            return WeekDayTimes.Any(dt => dt.IsEnabled);
        }
        
        /// <summary>
        /// 保存设置
        /// </summary>
        private void Save()
        {
            try
            {
                if (SelectedSchedule == null)
                    return;
                    
                // 更新选中计划的属性
                SelectedSchedule.Mode = SelectedMode;
                
                if (SelectedMode == ScheduleMode.DateRange)
                {
                    // 保存日期范围模式的数据
                    if (!StartDate.HasValue || !EndDate.HasValue)
                    {
                        MessageBox.Show("请设置有效的开始日期和结束日期！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    if (StartDate.Value > EndDate.Value)
                    {
                        MessageBox.Show("开始日期不能晚于结束日期！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    SelectedSchedule.StartDate = StartDate;
                    SelectedSchedule.EndDate = EndDate;
                    SelectedSchedule.DateRangeShutdownTime = DateRangeTime.TimeSpan;
                }
                else // WeekDay模式
                {
                    // 保存星期几模式的数据
                    SelectedSchedule.WeekDayShutdownTimes.Clear();
                    
                    foreach (var dayTime in WeekDayTimes)
                    {
                        if (dayTime.IsEnabled)
                        {
                            SelectedSchedule.WeekDayShutdownTimes[dayTime.DayOfWeek] = dayTime.Time.TimeSpan;
                        }
                    }
                    
                    if (SelectedSchedule.WeekDayShutdownTimes.Count == 0)
                    {
                        MessageBox.Show("请至少启用一天的关机时间！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
                // 保存所有计划到配置
                var config = ConfigService.GetConfig();
                config.Schedules = Schedules.ToList();
                ConfigService.SaveConfig(config);
                
                // 重启关机服务以应用新设置
                ShutdownService shutdownService = new ShutdownService();
                shutdownService.Start();
                
                MessageBox.Show("设置已保存！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // 触发窗口关闭事件
                WindowClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存设置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogService.LogError("保存设置失败", ex);
            }
        }
        
        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void Close()
        {
            WindowClose?.Invoke(this, EventArgs.Empty);
        }
    }
    
    /// <summary>
    /// 星期几时间视图模型
    /// </summary>
    public class DayTimeViewModel : ViewModelBase
    {
        private DayOfWeek _dayOfWeek;
        private string _dayName;
        private TimeWrapper _time;
        private bool _isEnabled;
        
        /// <summary>
        /// 星期几
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }
        
        /// <summary>
        /// 星期几名称
        /// </summary>
        public string DayName
        {
            get => _dayName;
            set => SetProperty(ref _dayName, value);
        }
        
        /// <summary>
        /// 关机时间
        /// </summary>
        public TimeWrapper Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dayOfWeek">星期几</param>
        /// <param name="dayName">星期几名称</param>
        public DayTimeViewModel(DayOfWeek dayOfWeek, string dayName)
        {
            DayOfWeek = dayOfWeek;
            DayName = dayName;
            Time = new TimeWrapper(new TimeSpan(21, 0, 0));
            IsEnabled = false;
        }
    }
    
    /// <summary>
    /// TimeSpan包装类，提供Hours和Minutes属性用于数据绑定
    /// </summary>
    public class TimeWrapper : ViewModelBase
    {
        private TimeSpan _timeSpan;
        private int _hours;
        private int _minutes;
        
        /// <summary>
        /// 小时
        /// </summary>
        public int Hours
        {
            get => _hours;
            set
            {
                if (SetProperty(ref _hours, value))
                {
                    UpdateTimeSpan();
                }
            }
        }
        
        /// <summary>
        /// 分钟
        /// </summary>
        public int Minutes
        {
            get => _minutes;
            set
            {
                if (SetProperty(ref _minutes, value))
                {
                    UpdateTimeSpan();
                }
            }
        }
        
        /// <summary>
        /// 底层TimeSpan对象
        /// </summary>
        public TimeSpan TimeSpan
        {
            get => _timeSpan;
            set
            {
                if (_timeSpan != value)
                {
                    _timeSpan = value;
                    _hours = _timeSpan.Hours;
                    _minutes = _timeSpan.Minutes;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Hours));
                    OnPropertyChanged(nameof(Minutes));
                }
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timeSpan">初始TimeSpan</param>
        public TimeWrapper(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }
        
        /// <summary>
        /// 更新TimeSpan值
        /// </summary>
        private void UpdateTimeSpan()
        {
            _timeSpan = new TimeSpan(_hours, _minutes, 0);
            OnPropertyChanged(nameof(TimeSpan));
        }
        
        /// <summary>
        /// 隐式转换为TimeSpan
        /// </summary>
        public static implicit operator TimeSpan(TimeWrapper wrapper)
        {
            return wrapper.TimeSpan;
        }
    }
} 