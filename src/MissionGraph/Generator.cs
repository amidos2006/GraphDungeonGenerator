using System.Collections.Generic;
using System.IO;
using System;

namespace ObstacleTowerGeneration.MissionGraph{
    /// <summary>
    /// Generate the mission graph from the starting graph using a recipe file
    /// </summary>
    class Generator
    {
        /// <summary>
        /// Random variable that is used to help generate different random expansions from the graph everytime you run
        /// </summary>
        private Random random;
        /// <summary>
        /// A dictionary for all the patterns that are loaded from the folders
        /// </summary>
        private Dictionary<string, Pattern> patterns;

        /// <summary>
        /// Constructor for the mission graph generator
        /// </summary>
        /// <param name="random">using one random variable to be able to replicate the results by fixing the seed</param>
        public Generator(Random random){
            this.random = random;
            this.patterns = new Dictionary<string, Pattern>();
        }

        /// <summary>
        /// Load all the different patterns that can be used in the recipe file
        /// </summary>
        /// <param name="foldername">the folder that contains all the patterns</param>
        public void loadPatterns(string foldername){
            string[] folders = Directory.GetDirectories(foldername);
            foreach(string f in folders){
                string key = f.Split(foldername)[1].Trim().ToLower();
                this.patterns.Add(key, new Pattern(this.random));
                this.patterns[key].loadPattern(f + "/");
            }
        }

        /// <summary>
        /// Generate the mission graph and return it
        /// </summary>
        /// <param name="startname">the starting graph which is usually a start and exist</param>
        /// <param name="recipename">the recipe file that need to be exectuted to generate the graph</param>
        /// <param name="maxConnections">the maximum number of connection any node should have (4 is the default to help the layout generation)</param>
        /// <returns>the generated mission graph</returns>
        public Graph GenerateDungeon(string startname, string recipename, int maxConnections = 4){
            Graph graph = new Graph();
            graph.loadGraph(startname);

            List<Recipe> recipes = Recipe.loadRecipes(recipename);
            foreach(Recipe r in recipes){
                if(this.patterns.ContainsKey(r.action.ToLower())){
                    int count = this.random.Next(r.maxTimes - r.minTimes + 1) + r.minTimes;
                    for(int i=0; i<count; i++){
                        this.patterns[r.action.ToLower()].applyPattern(graph, maxConnections);
                    }
                }
                else{
                    Pattern randomPattern = null;
                    foreach(Pattern p in this.patterns.Values){
                        if(randomPattern == null || this.random.NextDouble() < 0.3){
                            randomPattern = p;
                        }
                    }
                    randomPattern.applyPattern(graph, maxConnections);
                }
            }
            return graph;
        }
    }
}
