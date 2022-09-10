using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnergyRoom.Models
{
    public class Users : INotifyPropertyChanged
    {
        private int _id { get; set; }
        private string _name { get; set; }
        private string _phone { get; set; }
        private string _email { get; set; }
        private string _username { get; set; }
        private int _userTypeId { get; set; }
        private string _userTypeDescr { get; set; }

        public int ID
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
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
        public string Email
        {
            get => _email;
            set
            {
                if (value == _email) return;
                _email = value;
                OnPropertyChanged();
            }
        }
        public string Username
        {
            get => _username;
            set
            {
                if (value == _username) return;
                _username = value;
                OnPropertyChanged();
            }
        }

        public int UserTypeId
        {
            get => _userTypeId;
            set
            {
                if (value == _userTypeId) return;
                _userTypeId = value;
                OnPropertyChanged();
            }
        }

        public string UserTypeDescr
        {
            get => _userTypeDescr;
            set
            {
                if (value == _userTypeDescr) return;
                _userTypeDescr = value;
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