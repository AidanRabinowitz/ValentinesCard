using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ValentineApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IAudioManager audioManager;
        private IAudioPlayer currentAudioPlayer;
        private bool isAlertShowing = false;

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();
            this.audioManager = audioManager;

            // Attach the event handlers to the Yes and No buttons
            yesButton.Clicked += OnYesClicked;
            noButton.Clicked += OnNoClicked;
        }

        private async void OnYesClicked(object sender, EventArgs e)
        {
            await PlayAudio("HappySong.mp3");
        }

        private async void OnNoClicked(object sender, EventArgs e)
        {
            await PlayAudio("SadSong.mp3");
        }

        private async Task PlayAudio(string fileName)
        {
            try
            {
                // Stop the currently playing audio, if any
                await StopCurrentAudio();

                var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                currentAudioPlayer = audioManager.CreatePlayer(stream);

                currentAudioPlayer.Play();
                if (fileName == "SadSong.mp3")
                {
                    if (!isAlertShowing)
                    {
                        isAlertShowing = true;
                        await DisplayAlert("Sadness", "Sad kitty", "No more buttons. Only sadness");
                        isAlertShowing = false;
                    }
                }
                else
                {
                    if (!isAlertShowing)
                    {
                        isAlertShowing = true;
                        await DisplayAlert("Yay!", "You said yes! Happy Valentine's Day!", "OK");
                        isAlertShowing = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        }

        private async Task StopCurrentAudio()
        {
            // Stop the current audio player if it exists
            currentAudioPlayer?.Stop();
            currentAudioPlayer?.Dispose();

            // Introduce a short delay to ensure audio player disposal
            await Task.Delay(500);
        }
    }
}
