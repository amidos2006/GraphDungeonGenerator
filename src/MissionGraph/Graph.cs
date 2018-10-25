using System.Collections.Generic;
using System.IO;
using System;

namespace ObstacleTowerGeneration.MissionGraph{
    class Graph{
        public List<Node> nodes{
            set;
            get;
        }

        public int relativeAccess{
            set;
            get;
        }


        public Graph(){
            this.nodes = new List<Node>();
            this.relativeAccess = 0;
        }

        public void loadGraph(string filename){
            Dictionary<int, Node> nodeDictionary = new Dictionary<int, Node>();
            using (StreamReader r = new StreamReader(filename))
            {
                string[] text = r.ReadToEnd().Split("\n");
                string type = "nodes";
                foreach (string line in text)
                {
                    if (line.Trim() == "nodes" || line == "edges" || line == "roots")
                    {
                        type = line.Trim();
                        continue;
                    }
                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }
                    string[] parts = line.Split(",");
                    switch (type)
                    {
                        case "nodes":
                            int id = int.Parse(parts[0]);
                            int accessLevel = int.Parse(parts[1]);
                            NodeType nodeType = (NodeType)Enum.Parse(typeof(NodeType), parts[2].Trim());
                            nodeDictionary.Add(id, new Node(id, accessLevel, nodeType));

                            break;
                        case "edges":
                            int id1 = int.Parse(parts[0]);
                            int id2 = int.Parse(parts[1]);
                            nodeDictionary[id1].connectTo(nodeDictionary[id2]);
                            break;
                    }
                }
            }
            foreach (Node node in nodeDictionary.Values)
            {
                this.nodes.Add(node);
            }
        }

        public int getNumConnections(Node node){
            int result = node.getChildren().Count;
            List<Node> queue = new List<Node>();
            queue.Add(node);
            HashSet<Node> visited = new HashSet<Node>();
            while(queue.Count > 0){
                Node current = queue[0];
                queue.Remove(current);
                if(visited.Contains(current)){
                    continue;
                }
                visited.Add(current);
                if(current == node){
                    result += 1;
                }
                foreach(Node child in current.getChildren()){
                    queue.Add(child);
                }
            }
            return result - 1;
        }

        public int getNodeIndex(Node n){
            return this.nodes.IndexOf(n);
        }

        public List<Graph> getPermutations(int size)
        {
            List<int> indeces = new List<int>();
            for (int i = 0; i < this.nodes.Count; i++)
            {
                indeces.Add(i);
            }
            List<List<int>> integerPermutations = Helper.getPermutations(indeces, size);
            List<Graph> nodePermutations = new List<Graph>();
            foreach (List<int> list in integerPermutations)
            {
                Graph temp = new Graph();
                for (int i = 0; i < list.Count; i++)
                {
                    temp.nodes.Add(this.nodes[list[i]]);
                }
                nodePermutations.Add(temp);
            }
            return nodePermutations;
        }

        public int getHighestAccessLevel(){
            int maxValue = 0;
            foreach(Node n in nodes){
                if(n.accessLevel > maxValue){
                    maxValue = n.accessLevel;
                }
            }
            return maxValue;
        }

        public bool checkSimilarity(Graph graph)
        {
            for (int i = 0; i < this.nodes.Count; i++)
            {
                if (!this.nodes[i].isNodeEqual(graph.nodes[i], this.relativeAccess - graph.relativeAccess))
                {
                    return false;
                }
                List<Node> pChild = this.nodes[i].getFilteredChildren(this);
                List<Node> gChild = graph.nodes[i].getFilteredChildren(graph);
                if (gChild.Count != pChild.Count)
                {
                    return false;
                }
                List<int> testing = new List<int>();
                for (int j = 0; j < pChild.Count; j++)
                {
                    int pIndex = this.getNodeIndex(pChild[j]);
                    testing.Add(pIndex);
                }
                for (int j = 0; j < gChild.Count; j++)
                {
                    int gIndex = this.getNodeIndex(pChild[j]);
                    if (!testing.Remove(gIndex))
                    {
                        return false;
                    }
                }
                if (testing.Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString(){
            string result = "Graph:\n";
            foreach (Node n in this.nodes)
            {
                result += "\t" + n.ToString();
            }
            return result;
        }
    }
}