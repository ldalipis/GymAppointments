﻿using System.ComponentModel;
using Xamarin.Forms;

namespace EnergyRoom.Models
{
    public class FacebookProfile : INotifyPropertyChanged
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public UriImageSource PictureSrc { get; set; }
        public string Picture { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
