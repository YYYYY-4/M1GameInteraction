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
    public class PlayerSave
    {
        private string _name;
        private int _clearedLevels = 0;
        private int _saveSlot;
        private string _path;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int ClearedLevels
        {
            get { return _clearedLevels; }
            set { _clearedLevels = value; }
        }
        public int SaveSlot
        {
            get { return _saveSlot; }
            set { _saveSlot = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        
        /// <summary>
        /// Empty constructor for the creation of PlayerSave objects based on JSON files
        /// </summary>
        public PlayerSave()
        {

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
                _name = name;
            }
            else
            {
                _name = name;
            }
           
            _saveSlot = saveSlot;
            _path = GetSavePath(saveSlot);
        }

        /// <summary>
        /// Saves playerSave data to a JSON file
        /// </summary>
        public void Save()
        {
            string cl = _clearedLevels.ToString();
            //List<string> data = new List<string>() { _name, cl };
            string json = JsonSerializer.Serialize(this);
            if (!File.Exists(_path))
            {
                var file = File.Create(_path);
                file.Close();
                File.WriteAllText(_path, json);
            }
            else 
            {
                File.WriteAllText(_path, json);
            }
        }

        /// <summary>
        /// Loads a savefile from JSON
        /// </summary>
        /// <param name="saveIndex"></param>
        /// <returns>PlayerSave object</returns>
        static public PlayerSave? Load(int saveIndex)
        {
            string path = GetSavePath(saveIndex);
            if (File.Exists(path))
            { 
                PlayerSave json = JsonSerializer.Deserialize<PlayerSave>(File.ReadAllText(path));
                return json;
            }
            return null;
        }

        public void Delete()
        {
            File.Delete(_path);
        }

        static string GetSavePath(int saveIndex)
        {
            string path = "Save" + saveIndex + ".json";
            return path;
        }
    }
}
