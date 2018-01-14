﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IntelligentApp.ViewModels
{
    public class Picture : ViewModel
    {
        private MediaFile _mediaFile;

        private string _image;
        public string Image
        {
            get => _image;
            set => this.SetValue(value, ref _image);
        }

        private bool _canAnalyze = false;
        public bool CanAnalyze
        {
            get => _canAnalyze;
            set => this.SetValue(value, ref _canAnalyze);
        }

        public Command TakePhotoCommand { get; set; }
        public Command PickPhotoCommand { get; set; }
        public Command AnalyzeCommand { get; set; }
        public Picture()
        {
            this.TakePhotoCommand = new Command(async () => await this.TakePhoto());
            this.PickPhotoCommand = new Command(async () => await this.PickPhoto());
            this.AnalyzeCommand = new Command(async () => await this.Analyze());
        }

        private async Task TakePhoto()
            => await Execute(() => CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = this.Title,
                Name = Guid.NewGuid().ToString()
            }));

        private async Task PickPhoto()
            => await Execute(() => CrossMedia.Current.PickPhotoAsync());

        private async Task Execute(Func<Task<MediaFile>> func)
        {
            try
            {
                this.CanAnalyze = false;
                this._mediaFile = await func();
                this.Image = this._mediaFile.Path;
                this.CanAnalyze = true;
            }
            catch(Exception ex)
            {
                await this.Message.DisplayAlert(this.Title, "Could not pick or take a photo...", "Ok");
                this.IsBusy = false;
            }
        }

        private async Task Analyze()
            => await this.Navigation.To<AnalyzePhoto>(this.Parameters);
    }
}