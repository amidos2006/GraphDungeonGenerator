using System;
using System.Collections.Generic;

namespace ObstacleTowerGeneration.LayoutGrammar{
    /// <summary>
    /// The map layout class
    /// </summary>
    class Map{
        /// <summary>
        /// random variable used to select cells randomly for expansions
        /// </summary>
        private Random random;
        /// <summary>
        /// A list of all the possible direction to connect cells
        /// </summary>
        /// <value></value>
        private List<int[]> directions = new List<int[]>{new int[]{-1, 0}, new int[]{1, 0}, 
            new int[]{0, -1}, new int[]{0, 1}};
        /// <summary>
        /// A dictionary of all possible openings from each location
        /// </summary>
        private Dictionary<string, List<OpenNode>> openLocations;
        /// <summary>
        /// A dictionary of all used x,y locations
        /// </summary>
        private Dictionary<string, Cell> usedSpaces;

        /// <summary>
        /// Constructor that creates an empty layout
        /// </summary>
        /// <param name="random">the random variable that is used in generation</param>
        public Map(Random random){
            this.random = random;
            this.usedSpaces = new Dictionary<string, Cell>();
            this.openLocations = new Dictionary<string, List<OpenNode>>();
        }

        /// <summary>
        /// Start the map by adding the first cell that correspond to the starting node of the mission graph
        /// </summary>
        /// <param name="node">the starting cell that has only to be connected randomly from one direction</param>
        public void initializeCell(MissionGraph.Node node){
            Cell start = new Cell(0, 0, CellType.Normal, node);
            this.usedSpaces.Add(start.getLocationString(), start);
            this.openLocations.Add("0,0", new List<OpenNode>());
            int[] randDir = this.directions[this.random.Next(this.directions.Count)];
            this.openLocations["0,0"].Add(new OpenNode(randDir[0], randDir[1], start));
            // foreach(int[] dir in this.directions){
            //     this.openLocations["0,0"].Add(new OpenNode(dir[0], dir[1], start));
            // }
        }

        /// <summary>
        /// Get an empty location that has a specific access level and connected to a certain node
        /// </summary>
        /// <param name="parentID">the node id that need to be connected to</param>
        /// <param name="accessLevel">the access level that the cell need to be at</param>
        /// <returns>the empty location that has a certain parent id and access level</returns>
        private OpenNode getWorkingLocation(int parentID, int accessLevel){
            if(!this.openLocations.ContainsKey(parentID + "," + accessLevel)){
                return null;
            }
            Helper.shuffleList(this.random, this.openLocations[parentID + "," + accessLevel]);
            OpenNode selected = null;
            foreach (OpenNode loc in this.openLocations[parentID + "," + accessLevel])
            {
                if (!this.usedSpaces.ContainsKey(loc.x + "," + loc.y))
                {
                    selected = loc;
                    break;
                }
            }
            return selected;
        }

        /// <summary>
        /// Adding a new cell in the map and modifying the used spaces and open spaces
        /// </summary>
        /// <param name="c">the new cell that is being added</param>
        /// <param name="nextAccess">the new access level based on that cell</param>
        /// <param name="parentID">the parent id that new cell</param>
        private void addNewNode(Cell c, int nextAccess, int parentID){
            this.usedSpaces.Add(c.getLocationString(), c);
            if(!this.openLocations.ContainsKey(parentID + "," + nextAccess)){
                this.openLocations.Add(parentID + "," + nextAccess, new List<OpenNode>());
            }
            foreach(int[] dir in this.directions){
                int newX = c.x + dir[0];
                int newY = c.y + dir[1];
                if(!this.usedSpaces.ContainsKey(newX + "," + newY)){
                    this.openLocations[parentID + "," + nextAccess].Add(new OpenNode(newX, newY, c));
                }
            }
        }

        /// <summary>
        /// Get the cell that has a certain mission graph node id
        /// </summary>
        /// <param name="id">the id of the mission graph node</param>
        /// <returns>the cell that have a mission graph node id</returns>
        public Cell getCell(int id){
            foreach(Cell c in this.usedSpaces.Values){
                if(c.node != null && c.node.id == id){
                    return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a path between the "from" cell to the "to" cell without changing the access level
        /// </summary>
        /// <param name="from">the starting cell</param>
        /// <param name="to">the ending cell</param>
        /// <param name="accessLevel">the access level of the successor cells in the path</param>
        /// <returns>a list of (x,y) locations that connect between "from" cell to "to" cell</returns>
        public List<int[]> getDungeonPath(Cell from, Cell to, int accessLevel){
            List<TreeNode> queue = new List<TreeNode>();
            queue.Add(new TreeNode(from.x, from.y, null, to.x, to.y));
            HashSet<string> visited = new HashSet<string>();
            while (queue.Count > 0)
            {
                queue.Sort();
                TreeNode current = queue[0];
                queue.Remove(current);
                if (current.x == to.x && current.y == to.y)
                {
                    return current.getPath();
                }
                if (from.parent != null && current.x == from.parent.x && current.y == from.parent.y){
                    continue;
                }
                if (visited.Contains(current.x + "," + current.y))
                {
                    continue;
                }
                if (!this.usedSpaces.ContainsKey(current.x + "," + current.y)){
                    continue;
                }
                if(this.usedSpaces[current.x + "," + current.y].type == CellType.Connection){
                        continue;
                }
                if (this.usedSpaces[current.x + "," + current.y].node.accessLevel != accessLevel)
                {
                    continue;
                }
                
                visited.Add(current.x + "," + current.y);
                foreach (int[] dir in directions)
                {
                    if(this.usedSpaces[current.x + "," + current.y].getDoor(-dir[0], -dir[1]) != 0){
                        int newX = current.x + dir[0];
                        int newY = current.y + dir[1];
                        queue.Add(new TreeNode(newX, newY, current, to.x, to.y));
                    }
                }
            }
            return new List<int[]>();
        }

        /// <summary>
        /// Get a list of all open spaces that are needed to be transformed to cells to maintain connection between "from" cell to "to" cell
        /// </summary>
        /// <param name="from">the starting cell</param>
        /// <param name="to">the ending cell</param>
        /// <param name="maxIterations">the number of times that algorithm should try before failing</param>
        /// <returns>the empty locations that need to be converted to "conncet" cells</returns>
        private List<int[]> getConnectionPoints(Cell from, Cell to, int maxIterations){
            int accessLevel = Math.Min(from.node.accessLevel, to.node.accessLevel);
            List<TreeNode> queue = new List<TreeNode>();
            foreach (int[] dir in directions)
            {
                int newX = from.x + dir[0];
                int newY = from.y + dir[1];
                queue.Add(new TreeNode(newX, newY, new TreeNode(from.x, from.y, null, to.x, to.y), to.x, to.y));
            }
            HashSet<string> visited = new HashSet<string>();
            visited.Add(from.x + "," + from.y);
            while (queue.Count > 0)
            {
                queue.Sort();
                TreeNode current = queue[0];
                queue.Remove(current);
                if (current.x == to.x && current.y == to.y){
                    return current.getPath();
                }
                if(this.usedSpaces.ContainsKey(current.x + "," + current.y) && 
                    this.usedSpaces[current.x + "," + current.y].type == CellType.Normal){
                    if(this.usedSpaces[current.x + "," + current.y].node.accessLevel > accessLevel){
                        continue;
                    }
                    List<int[]> dungeonPath = this.getDungeonPath(this.usedSpaces[current.x + "," + current.y], to, accessLevel);
                    if(dungeonPath.Count > 0){
                        return current.getPath();
                    }
                }
                if(from.parent != null && current.x == from.parent.x && current.y == from.parent.y){
                    continue;
                }
                if (visited.Contains(current.x + "," + current.y)){
                    continue;
                }
                if(visited.Count > maxIterations){
                    return new List<int[]>();
                }
                visited.Add(current.x + "," + current.y);
                foreach (int[] dir in directions)
                {
                    int newX = current.x + dir[0];
                    int newY = current.y + dir[1];
                    queue.Add(new TreeNode(newX, newY, current, to.x, to.y));
                }
            }
            return new List<int[]>();
        }

        /// <summary>
        /// Create the "connect" cells to connect "from" cell to "to" cell
        /// </summary>
        /// <param name="from">the starting cell</param>
        /// <param name="to">the end cell</param>
        /// <param name="maxIterations">the maximum number of trials before failing</param>
        /// <returns>True if the connection succeeded and False otherwise</returns>
        public bool makeConnection(Cell from, Cell to, int maxIterations){
            List<int[]> points = this.getConnectionPoints(from, to, maxIterations);
            if(points.Count == 0){
                return false;
            }
            foreach(int[] p in points){
                if(!this.usedSpaces.ContainsKey(p[0] + "," + p[1])){
                    Cell currentCell = new Cell(p[0], p[1], CellType.Connection, null);
                    this.usedSpaces.Add(currentCell.getLocationString(), currentCell);
                }
            }
            for(int i=1; i<points.Count; i++){
                int[] p = points[i];
                Cell previousCell = this.usedSpaces[points[i - 1][0] + "," + points[i - 1][1]];
                Cell currentCell = this.usedSpaces[p[0] + "," + p[1]];
                DoorType door = DoorType.Open;
                if (previousCell.node != null)
                {
                    if(previousCell.node.type == MissionGraph.NodeType.Lever){
                        door = DoorType.LeverLock;
                    }
                }
                if(currentCell.getDoor(currentCell.x - previousCell.x, currentCell.y - previousCell.y) == 0){
                    currentCell.connectCells(previousCell, door);
                }
                
            }
            return true;
        }

        /// <summary>
        /// Add a new cell to the layout that correspond to a certain node in the mission graph
        /// </summary>
        /// <param name="node">corresponding node in the mission graph</param>
        /// <param name="parentID">the id of the parent that the new cell should be connected to</param>
        /// <returns>True if it succeed and False otherwise</returns>
        public bool addCell(MissionGraph.Node node, int parentID){
            if(node.type == MissionGraph.NodeType.Lock){
                OpenNode selected = this.getWorkingLocation(parentID, node.accessLevel - 1);
                if(selected == null){
                    return false;
                }
                Cell newCell = new Cell(selected.x, selected.y, CellType.Normal, node);
                newCell.connectCells(selected.parent, DoorType.KeyLock);
                newCell.parent = selected.parent;
                this.addNewNode(newCell, node.accessLevel, node.id);
            }
            else if(node.type == MissionGraph.NodeType.Puzzle){
                OpenNode selected = this.getWorkingLocation(parentID, node.accessLevel);
                if(selected == null){
                    return false;
                }
                Cell newCell = new Cell(selected.x, selected.y, CellType.Normal, node);
                newCell.connectCells(selected.parent, DoorType.Open);
                newCell.parent = selected.parent;
                this.addNewNode(newCell, node.accessLevel + 1, node.id);
            }
            else if(node.type == MissionGraph.NodeType.Lever){
                OpenNode selected = this.getWorkingLocation(parentID, node.accessLevel);
                if (selected == null){
                    return false;
                }
                Cell newCell = new Cell(selected.x, selected.y, CellType.Normal, node);
                newCell.connectCells(selected.parent, DoorType.Open);
                newCell.parent = selected.parent;
                this.usedSpaces.Add(newCell.getLocationString(), newCell);
            }
            else{
                OpenNode selected = this.getWorkingLocation(parentID, node.accessLevel);
                if(selected == null){
                    return false;
                }
                Cell newCell = new Cell(selected.x, selected.y, CellType.Normal, node);
                if(selected.parent.node.type == MissionGraph.NodeType.Puzzle){
                    newCell.connectCells(selected.parent, DoorType.PuzzleLock);
                }
                else if(selected.parent.node.type == MissionGraph.NodeType.Lever){
                    newCell.connectCells(selected.parent, DoorType.LeverLock);
                }
                else{
                    newCell.connectCells(selected.parent, DoorType.Open);
                }
                if(node.getChildren().Count == 0){
                    this.usedSpaces.Add(newCell.getLocationString(), newCell);
                }
                else{
                    this.addNewNode(newCell, node.accessLevel, parentID);
                }
                newCell.parent = selected.parent;
            }
            return true;
        }

        /// <summary>
        /// return a 2D array of cells where the cells are placed in a grid
        /// </summary>
        /// <returns>2D array of the cells</returns>
        public Cell[,] get2DMap(){
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;
            foreach(Cell c in this.usedSpaces.Values){
                if(c.x < minX){
                    minX = c.x;
                }
                if(c.y < minY){
                    minY = c.y;
                }
                if(c.x > maxX){
                    maxX = c.x;
                }
                if(c.y > maxY){
                    maxY = c.y;
                }
            }
            Cell[,] result = new Cell[maxX - minX + 1, maxY - minY + 1];
            foreach (Cell c in this.usedSpaces.Values){
                Cell clone = c.clone();
                clone.x = c.x - minX;
                clone.y = c.y - minY;
                result[clone.x, clone.y] = clone;
            }
            return result;
        }

        /// <summary>
        /// Get a string represntation of the current layout
        /// </summary>
        /// <returns>a string of the current map layout</returns>
        public override string ToString(){
            string nullCell = "     \n     \n     \n     \n     ";
            string result = "";
            Cell[,] result2D = this.get2DMap();
            for(int y=0; y<result2D.GetLength(1); y++){
                string[] parts = new string[5];
                for(int x=0; x<result2D.GetLength(0); x++){
                    string[] temp = nullCell.Split('\n');
                    if(result2D[x,y] != null){
                        temp = result2D[x,y].ToString().Split('\n');
                    }
                    for(int i=0; i<parts.Length; i++){
                        parts[i] += temp[i];
                    }
                }
                for(int i=0; i<parts.Length; i++){
                    result += parts[i] + "\n";
                }
            }
            return result;
        }
    }
}