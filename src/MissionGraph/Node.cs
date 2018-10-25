using System.Collections.Generic;
using System;

namespace ObstacleTowerGeneration.MissionGraph{
    class Node
    {
        public int id{
            set;
            get;
        }
        public int accessLevel{
            set;
            get;
        }
        public NodeType type{
            set;
            get;
        }

        protected List<Node> links;

        public Node(int id, int accessLevel, NodeType type){
            this.id = id;
            this.links = new List<Node>();
            this.type = type;
            this.accessLevel = accessLevel;
        }

        public List<Node> getChildren(){
            List<Node> result = new List<Node>();
            foreach(Node c in this.links){
                result.Add(c);
            }
            return result;
        }

        public List<Node> getFilteredChildren(Graph graph)
        {
            List<Node> children = this.getChildren();
            for (int i = 0; i < children.Count; i++)
            {
                int cIndex = graph.getNodeIndex(children[i]);
                if (cIndex == -1)
                {
                    children.RemoveAt(i);
                    i -= 1;
                }
            }
            return children;
        }

        public void adjustAccessLevel(int newLevel){
            if(this.accessLevel == newLevel){
                return;
            }
            this.accessLevel = newLevel;
            if(this.type != NodeType.Lever){
                List<Node> children = this.getChildren();
                foreach (Node c in children)
                {
                    if(c.type == NodeType.Lock || this.type == NodeType.Puzzle){
                        c.adjustAccessLevel(newLevel + 1);
                    }
                    else{
                        c.adjustAccessLevel(newLevel);
                    }
                }
            }
        }

        public void connectTo(Node node){
            this.links.Add(node);
        }

        public void removeLinks(Node node, bool twoWays=true){
            for(int i=0; i<this.links.Count; i++){
                if(this.links[i] == node){
                    this.links.RemoveAt(i);
                    i-=1;
                }
            }
            if (twoWays){
                node.removeLinks(this, false);
            }
        }

        public bool isNodeEqual(Node node, int accessModification = 0){
            if(this.accessLevel + accessModification == node.accessLevel){
                return (this.type == NodeType.Any || node.type == NodeType.Any || this.type == node.type);
            }
            return false;
        }

        public override string ToString(){
            string result = "Node " + this.id + " is " + this.type + "_" + this.accessLevel;
            List<Node> children = this.getChildren();
            if(children.Count > 0){
                result += " connections: ";
            }
            foreach (Node c in children)
            {
                result += c.id + " ";
            }
            result += "\n";
            return result;
        }
    }
}
