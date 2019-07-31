
namespace ObstacleTowerGeneration.MissionGraph{
    /// <summary>
    /// Different types of nodes in the mission graph
    /// </summary>
    enum NodeType
    {
        ///Only nodes that are just created have that type
        None= ' ',
        ///normal traversing node
        Normal = 'N',
        ///nodes that increase the access level and need a key to unlock
        Lock = 'L',
        ///nodes that have a key that is used to unlock locked nodes
        Key = 'K',
        ///lever connect high level access nodes to low level access nodes
        Lever = 'V',
        ///puzzle nodes increase the access level where player need to beat a sokoban puzzle to unlock
        Puzzle = 'P',
        ///used in pattern matching which means it doesn't matter the node type
        Any = ' ',
        ///The starting node
        Start = 'S',
        ///The goal node
        End = 'E'
    }
}
