using System.Collections.Generic;
using System.IO;

namespace ObstacleTowerGeneration.MissionGraph{
    /// <summary>
    /// A class for the action of applying the patterns any number of times
    /// </summary>
    class Recipe{
        /// <summary>
        /// the current pattern to be applied
        /// </summary>
        public string action{
            set;
            get;
        }
        /// <summary>
        /// minimum number of times to apply the pattern
        /// </summary>
        public int minTimes{
            set;
            get;
        }
        /// <summary>
        /// the maximum number of times to apply the pattern
        /// </summary>
        public int maxTimes{
            set;
            get;
        }

        /// <summary>
        /// constructor for the recipe class
        /// </summary>
        /// <param name="action">the current pattern to be executed</param>
        /// <param name="minTimes">the minimum number of times to execute the pattern</param>
        /// <param name="maxTimes">the maximum number of times to execute the pattern</param>
        public Recipe(string action, int minTimes, int maxTimes){
            this.action = action;
            this.minTimes = minTimes;
            this.maxTimes = maxTimes;
        }

        /// <summary>
        /// Static function to load a list of all the recipe that need to be executed from a txt file
        /// </summary>
        /// <param name="filename">the txt file that contain the recipe list</param>
        /// <returns>A list of all the recipe that need to be applied in order</returns>
        public static List<Recipe> loadRecipes(string filename){
            List<Recipe> recipes = new List<Recipe>();

            using (StreamReader r = new StreamReader(filename))
            {
                string[] text = r.ReadToEnd().Split("\n");
                foreach(string line in text){
                    string[] parts = line.Split(",");
                    if(parts[0].Trim().ToLower() == "any"){
                        recipes.Add(new Recipe(parts[0].Trim(), 0, 0));
                    }
                    else if(parts[0].Trim()[0] == '#'){
                        continue;
                    }
                    else{
                        int minValue = 0;
                        int maxValue = 0;
                        if(parts.Length == 1){
                            minValue = 1;
                            maxValue = 1;
                        }
                        else if(parts.Length == 2){
                            minValue = int.Parse(parts[1]);
                            maxValue = minValue;
                        }
                        else{
                            minValue = int.Parse(parts[1]);
                            maxValue = int.Parse(parts[2]);
                        }
                        recipes.Add(new Recipe(parts[0].Trim(), minValue, maxValue));
                    }
                }
            }

            return recipes;
        }
    }
}