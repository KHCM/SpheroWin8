using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRHS2backend;


namespace DriveSample
{
    public class GamesList
    {
        public List<Game> SpheroGameList { get; set; }
    }


    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public class SampleGameDataGroup
    {
        public SampleGameDataGroup(String uniqueId, String title, String imagePath)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.ImagePath = imagePath;
            this.GameItems = new ObservableCollection<Game>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<Game> GameItems { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// 
    /// I need to Greate a collection of groups and items with objects from a List<>
    /// </summary>
    public sealed class SampleGameDataSource
    {
        private static SampleGameDataSource _sampleGameDataSource = new SampleGameDataSource();

        private ObservableCollection<SampleGameDataGroup> _gameGroups = new ObservableCollection<SampleGameDataGroup>();
        public ObservableCollection<SampleGameDataGroup> gameGroups
        {
            get { return this._gameGroups; }
        }

        public static async Task<IEnumerable<SampleGameDataGroup>> GetGameGroupsAsync()
        {
            await _sampleGameDataSource.GetSampleGameDataAsync();

            return _sampleGameDataSource.gameGroups;
        }

        public static async Task<SampleGameDataGroup> GetGameGroupAsync(string uniqueId)
        {
            await _sampleGameDataSource.GetSampleGameDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleGameDataSource.gameGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<Game> GetGameItemAsync(string gameId)
        {
            await _sampleGameDataSource.GetSampleGameDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleGameDataSource.gameGroups.SelectMany(group => group.GameItems).Where((item) => item.GameId.Equals(gameId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetSampleGameDataAsync()
        {
            if (this._gameGroups.Count != 0)
                return;

            if (App.Current.AllGames.Count != 0)
            {
                SampleGameDataGroup spheroGroup = new SampleGameDataGroup("SpheroGroupID", "Sphero Games", "/Assets/Logo.scale-100.png");
                SampleGameDataGroup droneGroup = new SampleGameDataGroup("DroneGroupID", "Drone Games", "/Assets/Logo.scale-100.png");
                foreach (Game g in App.Current.AllGames)
                {
                    if (g.GameState == 0 || g.GameState == null)
                    {
                        if (g.DronePlayer.UserName == App.Current.AppUser.UserName)
                        {
                            droneGroup.GameItems.Add(g);
                        }
                        if (g.SpheroPlayer.UserName == App.Current.AppUser.UserName)
                        {
                            spheroGroup.GameItems.Add(g);
                        }
                    }

                }
                this.gameGroups.Add(spheroGroup);
                this.gameGroups.Add(droneGroup);

            }
     
        }
    }
}
