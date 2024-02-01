using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ValentineApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IAudioManager audioManager;
        private IAudioPlayer currentAudioPlayer;
        private bool isHappyAlertShowing = false;
        private bool isSadAlertShowing = false;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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
            await TransitionImage("happycat.jpg");
            await PlayAudio("HappySong.mp3");
        }

        private async void OnNoClicked(object sender, EventArgs e)
        {
            await TransitionImage("sadcat.jpg");
            await PlayAudio("SadSong.mp3");
        }

        private async Task TransitionImage(string imagePath)
        {
            if (backgroundImage == null)
            {
                // If the backgroundImage is null, create a new Image element
                backgroundImage = new Image { Aspect = Aspect.AspectFill };
                // Add the new Image to the Content of the ContentPage
                Content = new StackLayout { Children = { backgroundImage } };
            }

            await backgroundImage.FadeTo(0, 500); // Fade out the current image
            backgroundImage.Source = imagePath; // Change the image source
            await backgroundImage.FadeTo(1, 500); // Fade in the new image
        }


        private async Task PlayAudio(string fileName)
        {
            try
            {
                // Use semaphore to synchronize access to the audio player
                await semaphore.WaitAsync();
                StopCurrentAudio();

                var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                currentAudioPlayer = audioManager.CreatePlayer(stream);

                currentAudioPlayer.Play();

                // Check and show the appropriate alert
                if (fileName == "SadSong.mp3" && !isSadAlertShowing)
                {
                    isSadAlertShowing = true;
                    await DisplayAlert("Sadness", "Sad kitty", "No more buttons. Only sadness");
                    isSadAlertShowing = false;
                }
                else if (fileName == "HappySong.mp3" && !isHappyAlertShowing)
                {
                    isHappyAlertShowing = true;
                    await DisplayAlert("Yay!", "You said yes! Happy Valentine's Day!", "OK");
                    isHappyAlertShowing = false;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
            finally
            {
                // Release the semaphore
                semaphore.Release();
            }
        }

        private void StopCurrentAudio()
        {
            // Stop the current audio player if it exists
            currentAudioPlayer?.Stop();
            currentAudioPlayer?.Dispose();
        }
    }
}
