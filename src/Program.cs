using System;
using System.Collections.Generic;
using ObstacleTowerGeneration;

namespace ObstacleTowerGeneration
{
    struct DungeonResult{
        public MissionGraph.Graph missionGraph{
            set;
            get;
        }
        public LayoutGrammar.Map layoutMap{
            set;
            get;
        }

        public DungeonResult(MissionGraph.Graph missionGraph, LayoutGrammar.Map layoutMap){
            this.missionGraph = missionGraph;
            this.layoutMap = layoutMap;
        }
    }

    class Program
    {
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
                    if(resultMap != null){
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

        static void Main(string[] args)
        {
            DungeonResult result = generateDungeon();
            Console.WriteLine(result.missionGraph);
            Console.WriteLine();
            Console.WriteLine(result.layoutMap);
        }
    }
}
