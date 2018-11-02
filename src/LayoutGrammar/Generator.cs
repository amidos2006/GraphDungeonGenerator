using System;
using System.Collections.Generic;

namespace ObstacleTowerGeneration.LayoutGrammar{
    class Generator{
        private Random random;

        public Generator(Random random){
            this.random = random;
        }

        public Map generateDungeon(MissionGraph.Graph graph){
            Map result = new Map(this.random);
            result.initializeCell(graph.nodes[0]);
            #region  make initial dungeon
            List<MissionGraph.Node> open = new List<MissionGraph.Node>();
            Dictionary<MissionGraph.Node, int> parentIDs = new Dictionary<MissionGraph.Node, int>();
            foreach(MissionGraph.Node child in graph.nodes[0].getChildren()){
                open.Add(child);
                parentIDs.Add(child, 0);
            }
            HashSet<MissionGraph.Node> nodes = new HashSet<MissionGraph.Node>();
            nodes.Add(graph.nodes[0]);
            while(open.Count > 0){
                MissionGraph.Node current = open[0];
                open.RemoveAt(0);
                if(nodes.Contains(current)){
                    continue;
                }
                nodes.Add(current);
                if(!result.addCell(current, parentIDs[current])){
                    return null;
                }
                foreach (MissionGraph.Node child in current.getChildren())
                {
                    if(!parentIDs.ContainsKey(child)){
                        if(current.type == MissionGraph.NodeType.Lock || 
                            current.type == MissionGraph.NodeType.Puzzle){
                            parentIDs.Add(child, current.id);
                        }
                        else{
                            parentIDs.Add(child, parentIDs[current]);
                        }
                    }
                    open.Add(child);
                }
            }
            #endregion

            #region make lever connections
            open.Clear();
            nodes.Clear();
            open.Add(graph.nodes[0]);
            while(open.Count > 0){
                MissionGraph.Node current = open[0];
                open.RemoveAt(0);
                if(nodes.Contains(current)){
                    continue;
                }
                nodes.Add(current);
                foreach (MissionGraph.Node child in current.getChildren())
                {
                    Cell from = result.getCell(current.id);
                    Cell to = result.getCell(child.id);
                    if (current.type == MissionGraph.NodeType.Lever)
                    {
                        if(!result.makeConnection(from, to, nodes.Count * nodes.Count)){
                            return null;
                        }
                    }
                    open.Add(child);
                }
            }
            #endregion
            return result;
        }
    }
}