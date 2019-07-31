using System.Collections.Generic;
using System;

namespace ObstacleTowerGeneration.MissionGraph{
    /// <summary>
    /// The mission graph nodes
    /// </summary>
    class Node
    {
        /// <summary>
        /// unique id for the current Node
        /// </summary>
        public int id{
            set;
            get;
        }
        /// <summary>
        /// the access level of the node determine when should this node be accessed
        /// </summary>
        public int accessLevel{
            set;
            get;
        }
        /// <summary>
        /// The type of the node (puzzle node, normal, lock, key, etc)
        /// </summary>
        public NodeType type{
            set;
            get;
        }

        /// <summary>
        /// the children of that node
        /// </summary>
        protected List<Node> links;

        /// <summary>
        /// Constructor for the node class
        /// </summary>
        /// <param name="id">the unique id for the node</param>
        /// <param name="accessLevel">the current access level</param>
        /// <param name="type">the type of the node</param>
        public Node(int id, int accessLevel, NodeType type){
            this.id = id;
            this.links = new List<Node>();
            this.type = type;
            this.accessLevel = accessLevel;
        }

        /// <summary>
        /// get a copy of this node children
        /// </summary>
        /// <returns>a copy of the children</returns>
        public List<Node> getChildren(){
            List<Node> result = new List<Node>();
            foreach(Node c in this.links){
                result.Add(c);
            }
            return result;
        }

        /// <summary>
        /// get a subset of the children that are in a certain graph
        /// </summary>
        /// <param name="graph">the mission graph that need to be checked</param>
        /// <returns>a subset of the children that are in the input graph</returns>
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

        /// <summary>
        /// modify that node and all its children access level based on the new access level
        /// Lock/Puzzle Nodes increase the access level by 1 while 
        /// Lever nodes stop propagating the access level
        /// </summary>
        /// <param name="newLevel">the new access level value</param>
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

        /// <summary>
        /// add a new child to that node
        /// </summary>
        /// <param name="node">the new child node</param>
        public void connectTo(Node node){
            this.links.Add(node);
        }

        /// <summary>
        /// remove children from that node
        /// </summary>
        /// <param name="node">the child node to be removed</param>
        /// <param name="twoWays">make sure to remove all connections between both nodes</param>
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

        /// <summary>
        /// check if that node is similar to another node
        /// used in graph matching
        /// </summary>
        /// <param name="node">the other node to be matched</param>
        /// <param name="accessModification">a shift for the access level value for comparison</param>
        /// <returns>True if both node have same type and same access level</returns>
        public bool isNodeEqual(Node node, int accessModification = 0){
            if(this.accessLevel + accessModification == node.accessLevel){
                return (this.type == NodeType.Any || node.type == NodeType.Any || this.type == node.type);
            }
            return false;
        }

        /// <summary>
        /// return the information of the current node in form of string
        /// </summary>
        /// <returns>a string that represent the current node information</returns>
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
