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
                MissionGraph.Graph resultGraph = mg.GenerateDungeon("graphStart.txt", "recipe.txt");
                if(resultGraph == null){
                    continue;
                }
                Console.Write(resultGraph);
                if(!Helper.checkIsSolvable(resultGraph, resultGraph.nodes[0])){
                    continue;
                }
                LayoutGrammar.Generator lg = new LayoutGrammar.Generator(new Random());
                LayoutGrammar.Map resultMap = lg.generateDungeon(resultGraph);
                if(resultMap == null){
                    continue;
                }
                Console.Write(resultMap);
                break;
            }
        }
    }
}
