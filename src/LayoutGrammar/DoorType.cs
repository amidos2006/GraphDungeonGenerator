namespace ObstacleTowerGeneration.LayoutGrammar
{
    /// <summary>
    /// Different types of doors for each cell where each cell have 4 doors
    /// </summary>
    enum DoorType{
        ///Open door that doesn't need anything special to open
        Open = ' ',
        ///Need a key to unlock it
        KeyLock = 'x',
        ///Need to use a lever to open it, it is used to block entrance from low level access level 
        ///to high level access until that lever is open from the high level later to make shortcut in traversal
        LeverLock = 'v',
        ///Need to solve a sokoban level to unlock it
        PuzzleLock = 'z'
    }
}