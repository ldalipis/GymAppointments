using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace EnergyRoom.Models
{
    public class Meeting : INotifyPropertyChanged
    {
        private long _id;
        private int _eventId;
        private int _typeId;
        private int _userId;
        private string _fullName;
        private string _userName;
        private bool _isBooked;
        private int _eventCount;
        private string _location;
        private DateTime _startTime;
        private DateTime _endTime;
        private Color _color = Color.FromHex("#7BC667");
        private string _eventTitle = "Running";
        private string _eventIcon = "running.png";

        public long Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public bool IsBooked
        {
            get => _isBooked;
            set
            {
                if (value == _isBooked) return;
                _isBooked = value;
                OnPropertyChanged();
            }
        }

        public int EventCount
        {
            get => _eventCount;
            set
            {
                if (value == _eventCount) return;
                _eventCount = value;
                OnPropertyChanged();
            }
        }

        public int EventId
        {
            get => _eventId;
            set
            {
                if (value == _eventId) return;
                _eventId = value;
                OnPropertyChanged();
            }
        }

        public int TypeId
        {
            get => _typeId;
            set
            {
                if (value == _typeId) return;
                _typeId = value;
                Color = _typeId switch
                {
                    1 => Color.FromHex("#7BC667"),
                    2 => Color.FromHex("#9466F2"),
                    3 => Color.FromHex("#37AA97"),
                    4 => Color.FromHex("#4C3AB9"),
                    _ => Color.FromHex("#4C3AB9"),
                };
                EventTitle = _typeId switch
                {
                    1 => "TRX",
                    2 => "CrossFit",
                    3 => "Pilates",
                    4 => "Running",
                    _ => "Running",
                };
                EventIcon = _typeId switch
                {
                    1 => "trx.png",
                    2 => "crossfit.png",
                    3 => "pilates.png",
                    4 => "running.png",
                    _ => "running.png",
                };
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                if (value == _userId) return;
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (value == _fullName) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }
        
        public string UserName
        {
            get => _userName;
            set
            {
                if (value == _userName) return;
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                if (value == _location) return;
                _location = value;
                OnPropertyChanged();
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                if (value == _color) return;
                _color = value;
                OnPropertyChanged();
            }
        }

        public string EventTitle
        {
            get => _eventTitle;
            set
            {
                if (value == _eventTitle) return;
                _eventTitle = value;
                OnPropertyChanged();
            }
        }

        public string EventIcon
        {
            get => _eventIcon;
            set
            {
                if (value == _eventIcon) return;
                _eventIcon = value;
                OnPropertyChanged();
            }
        }

        #region DataFormSource
        private DateTime _date;
        [DataType(DataType.Date), Display(Name = "Date")]
        public DateTime Date
        {
            get => _date;
            set
            {
                if (value.Equals(_date)) return;

                _date = value;
                StartTime = new DateTime(_date.Year, _date.Month, _date.Day,
                    StartTime.Hour, StartTime.Minute, StartTime.Second);
            }
        }

        private DateTime _inputStartTime;
        [DataType(DataType.Time), Display(Name = "Start Time")]
        public DateTime InputStartTime
        {
            get => _inputStartTime;
            set
            {
                if (value.Equals(_inputStartTime)) return;

                _inputStartTime = value;
                StartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day,
                    _inputStartTime.Hour, _inputStartTime.Minute, _inputStartTime.Second);
            }
        }

        private DateTime _inputEndTime;
        [DataType(DataType.Time), Display(Name = "End Time")]
        public DateTime InputEndTime
        {
            get => _inputEndTime;
            set
            {
                if (value.Equals(_inputEndTime)) return;

                _inputEndTime = value;
                EndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, _inputEndTime.Hour,
                    _inputEndTime.Minute, _inputEndTime.Second);
            }
        }
        #endregion

        [Display(AutoGenerateField = false)]
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (value.Equals(_startTime)) return;
                _startTime = value;
                InputStartTime = value;
                //EndTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day,
                //    StartTime.Hour, StartTime.Minute, StartTime.Second).AddHours(1);
                OnPropertyChanged();
            }
        }

        [Display(AutoGenerateField = false)]
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (value.Equals(_endTime)) return;
                _endTime = value;
                InputEndTime = value;
                OnPropertyChanged();
            }
        }

        public Meeting(int eventId, int typeId, int userId, string fullName, string userName, DateTime startTime, DateTime endTime, int eventCount, bool isBooked = false)
        {
            EventId = eventId;
            TypeId = typeId;
            UserId = userId;
            FullName = fullName;
            UserName = userName;
            Date = startTime;
            StartTime = startTime;
            EndTime = endTime;
            //EndTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day,
            //    StartTime.Hour, StartTime.Minute, StartTime.Second).AddHours(1);
            IsBooked = isBooked;
            EventCount = eventCount;
        }

        public Meeting(DateTime startTime, DateTime endTime, int typeId = 1)
        {
            TypeId = typeId;
            Date = startTime;
            InputStartTime = startTime;
            InputEndTime = endTime;
            //InputEndTime = date.AddHours(1);
            IsBooked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
