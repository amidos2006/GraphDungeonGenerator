using System.Collections.Generic;
using System.IO;
using System;
using ObstacleTowerGeneration.MissionGraph;

namespace ObstacleTowerGeneration{
    static class Helper{
        private static List<T> copyList<T>(List<T> list){
            List<T> clone = new List<T>();
            foreach(T v in list){
                clone.Add(v);
            }
            return clone;
        }

        public static bool checkIsSolvable(Graph graph, Node root, HashSet<Node> visited = null){
            List<Node> queue = new List<Node>();
            List<Node> locks = new List<Node>();
            if(visited == null){
                visited = new HashSet<Node>();
            }
            visited.Add(root);
            foreach (Node c in root.getChildren()){
                queue.Add(c);
            }
            int keys = 0;
            while(queue.Count > 0){
                Node current = queue[0];
                queue.RemoveAt(0);
                if(visited.Contains(current)){
                    continue;
                }
                if(current.type == NodeType.Lock){
                    locks.Add(current);
                }
                else{
                    if (current.type == NodeType.End)
                    {
                        return true;
                    }
                    else if (current.type == NodeType.Key)
                    {
                        keys += 1;
                    }
                    visited.Add(current);
                    foreach (Node c in current.getChildren()){
                        queue.Add(c);
                    }
                }
            }
            int requiredKeys = 0;
            foreach(Node l in locks){
                HashSet<Node> newVisited = new HashSet<Node>();
                foreach(Node v in visited){
                    newVisited.Add(v);
                }
                if(!checkIsSolvable(graph, l, newVisited)){
                    requiredKeys += 1;
                }
            }
            if(requiredKeys < keys){
                return true;
            }
            return false;
        }

        public static List<List<int>> getPermutations(List<int> values, int size){
            List<List<int>> result = new List<List<int>>();
            if(size == 0){
                return result;
            }
            
            for(int i=0; i < values.Count; i++){
                List<int> clone = copyList<int>(values);
                clone.RemoveAt(i);
                List<List<int>> tempResult = getPermutations(clone, size - 1);
                if(tempResult.Count == 0){
                    result.Add(new List<int>());
                    result[result.Count - 1].Add(values[i]);
                }
                foreach(List<int> list in tempResult){
                    list.Insert(0, values[i]);
                    result.Add(list);
                }
            }
            
            return result;
        }

        public static void shuffleList<T>(Random random, List<T> list){
            for(int i=0; i<list.Count; i++){
                int newIndex = random.Next(list.Count);
                T temp = list[i];
                list[i] = list[newIndex];
                list[newIndex] = temp;
            }
        }
    }
}