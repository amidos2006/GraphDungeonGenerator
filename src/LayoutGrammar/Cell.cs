namespace ObstacleTowerGeneration.LayoutGrammar
{
    class Cell{
        public MissionGraph.Node node{
            set;
            get;
        }

        public CellType type{
            set;
            get;
        }

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

        private DoorType[] doorTypes;

        public Cell(int x, int y, CellType type, MissionGraph.Node node){
            this.x = x;
            this.y = y;
            this.type = type;
            this.node = node;
            this.doorTypes = new DoorType[4];
            this.parent = null;
        }

        public Cell clone(){
            Cell result = new Cell(this.x, this.y, this.type, this.node);
            result.parent = this.parent;
            for(int i=0; i<this.doorTypes.Length; i++){
                result.doorTypes[i] = this.doorTypes[i];
            }
            return result;
        }

        private int getDoorIndex(int dirX, int dirY){
            if(dirX <= -1){
                return 0;
            }
            if(dirX >= 1){
                return 1;
            }
            if(dirY <= -1){
                return 2;
            }
            if(dirY >= 1){
                return 3;
            }
            return -1;
        }

        public DoorType getDoor(int dirX, int dirY)
        {   
            return this.doorTypes[this.getDoorIndex(dirX, dirY)];
        }
        public void connectCells(Cell other, DoorType type, bool bothWays = true){
            int dirX = this.x - other.x;
            int dirY = this.y - other.y;
            this.doorTypes[this.getDoorIndex(dirX, dirY)] = type;
            if(bothWays){
                other.connectCells(this, type, false);
            }
        }

        public string getLocationString(){
            return this.x + "," + this.y;
        }

        public override string ToString(){
            string result = "";
            char northDoor = '-';
            switch(this.doorTypes[3]){
                case DoorType.Open:
                    northDoor = ' ';
                    break;
                case DoorType.KeyLock:
                    northDoor = 'x';
                    break;
                case DoorType.LeverLock:
                case DoorType.PuzzleLock:
                    northDoor = 'v';
                    if(this.node != null && (this.node.type == MissionGraph.NodeType.Puzzle || 
                        this.node.type == MissionGraph.NodeType.Lever)){
                        northDoor = '^';
                    }
                    break;
            }
            char southDoor = '-';
            switch (this.doorTypes[2])
            {
                case DoorType.Open:
                    southDoor = ' ';
                    break;
                case DoorType.KeyLock:
                    southDoor = 'x';
                    break;
                case DoorType.LeverLock:
                case DoorType.PuzzleLock:
                    southDoor = '^';
                    if (this.node != null && (this.node.type == MissionGraph.NodeType.Puzzle ||
                        this.node.type == MissionGraph.NodeType.Lever))
                    {
                        southDoor = 'v';
                    }
                    break;
            }
            char eastDoor = '|';
            switch (this.doorTypes[0])
            {
                case DoorType.Open:
                    eastDoor = ' ';
                    break;
                case DoorType.KeyLock:
                    eastDoor = 'x';
                    break;
                case DoorType.LeverLock:
                case DoorType.PuzzleLock:
                    eastDoor = '<';
                    if (this.node != null && (this.node.type == MissionGraph.NodeType.Puzzle ||
                        this.node.type == MissionGraph.NodeType.Lever))
                    {
                        eastDoor = '>';
                    }
                    break;
            }
            char westDoor = '|';
            switch (this.doorTypes[1])
            {
                case DoorType.Open:
                    westDoor = ' ';
                    break;
                case DoorType.KeyLock:
                    westDoor = 'x';
                    break;
                case DoorType.LeverLock:
                case DoorType.PuzzleLock:
                    westDoor = '>';
                    if (this.node != null && (this.node.type == MissionGraph.NodeType.Puzzle ||
                        this.node.type == MissionGraph.NodeType.Lever))
                    {
                        westDoor = '<';
                    }
                    break;
            }
            char roomType = 'C';
            if (this.type == CellType.Normal)
            {
                roomType = (char)this.node.type;
            }

            result += "+-" + northDoor + "-+" + "\n";
            result += "|   |" + "\n";
            result += westDoor + " " + roomType + " " + eastDoor + "\n";
            result += "|   |" + "\n";
            result += "+-" + southDoor + "-+";
            return result;
        }
    }
}