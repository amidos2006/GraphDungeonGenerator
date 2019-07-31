using System.Collections.Generic;
using System;
using System.IO;

namespace ObstacleTowerGeneration.MissionGraph{
    /// <summary>
    /// 
    /// </summary>
    class Pattern{
        /// <summary>
        /// random variable for selecting random match with the pattern
        /// </summary>
        private Random random;
        /// <summary>
        /// the input graph that need to be substituted 
        /// </summary>
        private Graph patternMatch;
        /// <summary>
        /// The output graph that will replace the matched input
        /// </summary>
        private List<Graph> patternApply;

        /// <summary>
        /// Constructor for the pattern class
        /// </summary>
        /// <param name="random">the same random object used by all the patterns to be able to replicate results</param>
        public Pattern(Random random){
            this.random = random;
            this.patternMatch = new Graph();
            this.patternApply = new List<Graph>();
        }

        /// <summary>
        /// Load the pattern folder where input.txt is the graph to be matched 
        /// and output files are all applicable results
        /// </summary>
        /// <param name="foldername">the folder path for the pattern matching</param>
        public void loadPattern(string foldername){
            this.patternMatch = new Graph();
            this.patternMatch.loadGraph(foldername + "input.txt");

            this.patternApply = new List<Graph>();
            string[] files = Directory.GetFiles(foldername, "output*");
            foreach(string f in files){
                Graph temp = new Graph();
                temp.loadGraph(f);
                this.patternApply.Add(temp);
            }
        }

        /// <summary>
        /// a fast way to check applicability of that pattern to the subgraph in that big graph
        /// </summary>
        /// <param name="graph">the full graph</param>
        /// <param name="subgraph">a subgraph from the full graph</param>
        /// <param name="maxValue">maximum number of connections from each node in the graph (set to 4 to help the layout generator later)</param>
        /// <returns>True if that pattern is applicable and false otherwise</returns>
        private bool checkPatternApplicable(Graph graph, Graph subgraph, int maxValue = 4){
            for(int i=0; i<this.patternMatch.nodes.Count; i++){
                foreach(Graph patternOutput in this.patternApply){
                    if (this.patternMatch.nodes[i].getChildren().Count < patternOutput.nodes[i].getChildren().Count){
                        if(graph.getNumConnections(graph.nodes[i]) >= maxValue){
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// apply that current pattern to a random subgraph in the mission graph (inplace change)
        /// </summary>
        /// <param name="graph">the current mission graph</param>
        /// <param name="maxConnection">maximum number of connection from any specific node (to help the layout generator)</param>
        public void applyPattern(Graph graph, int maxConnection = 4){
            List<Graph> permutations = graph.getPermutations(this.patternMatch.nodes.Count);
            Helper.shuffleList<Graph>(random, permutations);
            int maxAccessLevel = graph.getHighestAccessLevel();
            List<int> levels = new List<int>();
            for (int i = 0; i <= maxAccessLevel; i++)
            {
                levels.Add(i);
            }
            Helper.shuffleList(random, levels);
            Graph selectedSubgraph = new Graph();
            foreach (Graph subgraph in permutations)
            {
                foreach(int level in levels){
                    this.patternMatch.relativeAccess = level;
                    if (this.patternMatch.checkSimilarity(subgraph) && 
                        this.checkPatternApplicable(graph, subgraph, maxConnection))
                    {
                        selectedSubgraph = subgraph;
                        break;
                    }
                }
                if(selectedSubgraph.nodes.Count > 0){
                    break;
                }
            }
            if(selectedSubgraph.nodes.Count == 0){
                return;
            }
            foreach (Node n in selectedSubgraph.nodes)
            {
                List<Node> children = n.getFilteredChildren(selectedSubgraph);
                foreach (Node c in children)
                {
                    n.removeLinks(c);
                }
            }
            Graph selectedPattern = this.patternApply[this.random.Next(this.patternApply.Count)];
            for (int i = selectedSubgraph.nodes.Count; i < selectedPattern.nodes.Count; i++)
            {
                Node newNode = new Node(graph.nodes.Count, -1, selectedPattern.nodes[i].type);
                graph.nodes.Add(newNode);
                selectedSubgraph.nodes.Add(newNode);
            }
            for (int i = 0; i < selectedPattern.nodes.Count; i++)
            {
                selectedSubgraph.nodes[i].adjustAccessLevel(this.patternMatch.relativeAccess + selectedPattern.nodes[i].accessLevel);
                List<Node> children = selectedPattern.nodes[i].getChildren();
                foreach (Node c in children)
                {
                    int index = selectedPattern.getNodeIndex(c);
                    selectedSubgraph.nodes[i].connectTo(selectedSubgraph.nodes[index]);
                }
            }
        }
    }
}