using System;
using System.Collections.Generic;
using ObstacleTowerGeneration;

namespace ObstacleTowerGeneration
{
    /// <summary>
    /// A structure used to hold the result of the generation
    /// </summary>
    struct DungeonResult{
        /// <summary>
        /// get the mission graph
        /// </summary>
        public MissionGraph.Graph missionGraph{
            get;
        }
        /// <summary>
        /// get the level layout
        /// </summary>
        public LayoutGrammar.Map layoutMap{
            get;
        }

        /// <summary>
        /// A structure to hold both the mission graph and layout
        /// </summary>
        /// <param name="missionGraph">the generated mission graph</param>
        /// <param name="layoutMap">the level layout</param>
        public DungeonResult(MissionGraph.Graph missionGraph, LayoutGrammar.Map layoutMap){
            this.missionGraph = missionGraph;
            this.layoutMap = layoutMap;
        }
    }

    class Program
    {
        /// <summary>
        /// Generate the dungeon graph and layout
        /// </summary>
        /// <param name="totalTrials">number of trials used if any of the graph or map generation fails</param>
        /// <param name="graphTrials">number of trials to generate graph before consider it a fail</param>
        /// <param name="mapTrials">number of trials to generate the layout before consider it a fail</param>
        /// <returns>the generated graph and layout</returns>
        static DungeonResult generateDungeon(int totalTrials = 100, int graphTrials = 100, int mapTrials = 100){
            MissionGraph.Graph resultGraph = null;
            LayoutGrammar.Map resultMap = null;

            for(int i=0; i<totalTrials; i++){
                MissionGraph.Generator mg = new MissionGraph.Generator(new Random());
                mg.loadPatterns("grammar/");
                for (int j = 0; j < graphTrials; j++){
                    resultGraph = mg.GenerateDungeon("graphStart.txt", "graphRecipe.txt");
                    if(resultGraph != null && Helper.checkIsSolvable(resultGraph, resultGraph.nodes[0])){
                        break;
                    }
                    else{
                        resultGraph = null;
                    }
                }
                if(resultGraph == null){
                    continue;
                }

                LayoutGrammar.Generator lg = new LayoutGrammar.Generator(new Random());
                for(int j=0; j<mapTrials; j++){
                    resultMap = lg.generateDungeon(resultGraph);
                    if(resultMap != null && Helper.checkIsSolvable(resultMap.get2DMap(), resultMap.getCell(0))){
                        break;
                    }
                    else{
                        resultMap = null;
                    }
                }
                if(resultMap == null){
                    continue;
                }
                break;
            }
            return new DungeonResult(resultGraph, resultMap);
        }

        /// <summary>
        /// The main entry of the program where it test generating a dungeon
        /// </summary>
        /// <param name="args">command line arguments that are not used</param>
        static void Main(string[] args)
        {
            DungeonResult result = generateDungeon();
            Console.WriteLine(result.missionGraph);
            Console.WriteLine();
            Console.WriteLine(result.layoutMap);
        }
    }
}
