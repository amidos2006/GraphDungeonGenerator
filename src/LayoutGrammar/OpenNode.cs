namespace ObstacleTowerGeneration.LayoutGrammar{
    class OpenNode{
        public int x{
            set;
            get;
        }

        public int y{
            set;
            get;
        }

        public Cell parent{
            set;
            get;
        }

        public OpenNode(int x=0, int y=0, Cell parent=null){
            this.x = x;
            this.y = y;
            this.parent = parent;
        }
    }
}