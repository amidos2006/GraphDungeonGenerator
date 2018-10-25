using System.Collections.Generic;
using System.IO;

namespace ObstacleTowerGeneration.MissionGraph{
    class Recipe{
        public string action{
            set;
            get;
        }

        public int minTimes{
            set;
            get;
        }

        public int maxTimes{
            set;
            get;
        }

        public Recipe(string action, int minTimes, int maxTimes){
            this.action = action;
            this.minTimes = minTimes;
            this.maxTimes = maxTimes;
        }

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