using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Speaker> Speakers { get; set; }
        public Command GetSpeakersCommand { get; set; }

        private bool busy;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public bool IsBusy
        {
            get { return busy; }
            set
            {
                busy = value;
                OnPropertyChanged();
                //Update the can execute
                GetSpeakersCommand.ChangeCanExecute();
            }
        }

        public SpeakersViewModel()
        {
            Speakers = new ObservableCollection<Speaker>();
            GetSpeakersCommand = new Command(
                                    async () => await GetSpeakers(),
                                    () => !IsBusy);

        }

        private async Task GetSpeakers()
        {
            if (IsBusy)
                return;

            Exception error = null;
            try
            {
                IsBusy = true;
                using (var client = new HttpClient())
                {
                    //grab json from server
                    var json = await client.GetStringAsync("http://demo4404797.mockable.io/speakers");
                    var items = JsonConvert.DeserializeObject<List<Speaker>>(json);

                    Speaker miguelmg = new Speaker();
                    miguelmg.Id = "100";
                    miguelmg.Name = "Miguel Moreno";
                    miguelmg.Description = "I work at Viajes Para Ti S.L.U. I work on applications departement and I do the BuscoUnChollo application for Windows Platform. Also I work on web department as a backend developer of my company portals on PHP.";
                    miguelmg.Website = "https://github.com/miguelmg";
                    miguelmg.Title = "MiguelMG";
                    miguelmg.Avatar = "https://avatars1.githubusercontent.com/u/3147742?v=3&amp;s=460";

                    //Speakers.clear();
                    foreach (var item in items)
                        Speakers.Add(item);

                    Speakers.Add(miguelmg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                error = ex;
            }
            finally
            {
                IsBusy = false;
            }

            if (error != null)
                await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");

        }




    }
}
