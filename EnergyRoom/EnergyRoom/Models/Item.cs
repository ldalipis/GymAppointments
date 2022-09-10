using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnergyRoom.Models
{
    public class Item : INotifyPropertyChanged
    {
        private int _id;
        private byte[] _imageSrc;
        private string _date;
        private string _title;
        private string _fullName;
        private string _phone;
        private int _typeId;
        private int _userId;

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public byte[] ImageSrc
        {
            get => _imageSrc;
            set
            {
                if (value == _imageSrc) return;
                _imageSrc = value;
                OnPropertyChanged();
            }
        }

        public string Date
        {
            get => _date;
            set
            {
                if (value == _date) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
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

        public string Phone
        {
            get => _phone;
            set
            {
                if (value == _phone) return;
                _phone = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}