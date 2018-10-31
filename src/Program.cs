using System;
using System.Collections.Generic;
using ObstacleTowerGeneration;

namespace ObstacleTowerGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i=0; i<100; i++){
                MissionGraph.Generator mg = new MissionGraph.Generator(new Random());
                mg.loadPatterns("grammar/");
                MissionGraph.Graph resultGraph = null;
                for (int j = 0; j < 100; j++){
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
                Console.Write(resultGraph);

                LayoutGrammar.Generator lg = new LayoutGrammar.Generator(new Random());
                LayoutGrammar.Map resultMap = null;
                for(int j=0; j<100; j++){
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
                Console.Write(resultMap);
                break;
            }
        }
    }
}
