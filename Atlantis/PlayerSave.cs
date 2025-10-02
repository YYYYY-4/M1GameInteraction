using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Atlantis
{
    internal class PlayerSave
    {
        private string name;
        private int clearedLevels = 0;
        private int saveSlot;
        private string path;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        
        /// <summary>
        /// Creates playerSave object with a name and save slot
        /// </summary>
        /// <param name="name"></param>
        /// <param name="saveSlot"></param>
        public PlayerSave(string name, int saveSlot)
        {
            //Checks for comma's inside of player names and replaces them with dots when needed. This is needed for the highscore CSV file.
            if (name.Contains(","))
            {
                name.Replace(",", ".");
                this.name = name;
            }
            else
            {
                this.name = name;
            }
           
            this.saveSlot = saveSlot;
            path = GetSavePath(saveSlot);
        }

        /// <summary>
        /// Saves playerSave data to a JSON file
        /// </summary>
        public void Save()
        {
            string cl = clearedLevels.ToString();
            List<string> data = new List<string>() { name, cl };
            string json = JsonSerializer.Serialize(data);
            if (!File.Exists(path))
            {
                var file = File.Create(path).Dispose;
                File.WriteAllText(path, json);
            }
            else 
            {
                File.WriteAllText(path, json);
            }
        }

        /// <summary>
        /// Loads a savefile from JSON
        /// </summary>
        /// <param name="saveIndex"></param>
        /// <returns>PlayerSave object</returns>
        static public PlayerSave Load(int saveIndex)
        {
            string path = GetSavePath(saveIndex);
            PlayerSave json = JsonSerializer.Deserialize<PlayerSave>(File.ReadAllText(path));
            return json;
        }

        static string GetSavePath(int saveIndex)
        {
            string path = "Save" + saveIndex + ".json";
            return path;
        }
    }
}
