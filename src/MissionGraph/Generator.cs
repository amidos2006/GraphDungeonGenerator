using System.Collections.Generic;
using System.IO;
using System;

namespace ObstacleTowerGeneration.MissionGraph{
    class Generator
    {
        private Random random;
        private Dictionary<string, Pattern> patterns;

        public Generator(Random random){
            this.random = random;
            this.patterns = new Dictionary<string, Pattern>();
        }

        public void loadPatterns(string foldername){
            string[] folders = Directory.GetDirectories(foldername);
            foreach(string f in folders){
                string key = f.Split(foldername)[1].Trim().ToLower();
                this.patterns.Add(key, new Pattern(this.random));
                this.patterns[key].loadPattern(f + "/");
            }
        }

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
