using System;
using System.Collections;
using System.Collections.Generic;

namespace ObstacleTowerGeneration.LayoutGrammar {
    class TreeNode: IComparable<TreeNode>{
        public int x{
            set;
            get;
        }

        public int y{
            set;
            get;
        }

        public TreeNode parent{
            set;
            get;
        }

        private int endX{
            set;
            get;
        }

        private int endY{
            set;
            get;
        }

        public TreeNode(int x, int y, TreeNode parent, int endX, int endY){
            this.x = x;
            this.y = y;
            this.parent = parent;
            this.endX = endX;
            this.endY = endY;
        }

        public List<int[]> getPath(){
            List<int[]> result = new List<int[]>();
            TreeNode current = this;
            while(current != null){
                result.Add(new int[]{current.x, current.y});
                current = current.parent;
            }
            result.Reverse();
            return result;
        }

        public int CompareTo(TreeNode other)
        {
            return (Math.Abs(this.x - endX) + Math.Abs(this.y - endY)) - 
                (Math.Abs(other.x - endX) + Math.Abs(other.y - endY));
        }
    }
}