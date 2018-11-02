# Graph Dungeon Generator
## Description
This project generates game dungeons similar to the first zelda dungeons with only normal keys (no key items or breakable walls). The code is based on Joris Dormans' work in [dungeon generation](http://sander.landofsand.com/publications/Dormans_Bakkes_-_Generating_Missions_and_Spaces_for_Adaptable_Play_Experiences.pdf) for [Unexplored](https://store.steampowered.com/app/506870/Unexplored/).

## Features
- generate a graph of mission graph that describe how to solve the level similar to Mark Brown's Graph in [Boss Keys series](https://www.youtube.com/playlist?list=PLc38fcMFcV_ul4D6OChdWhsNsYY3NA5B2).
- generate a 2D grid of equal sized rooms for the mission graph similar to the [first zelda dungeon layout](http://www.gamasutra.com/view/feature/6582/learning_from_the_masters_level_.php?print=1). The number of generated rooms will be equal to the number of 

## Guide
The project consists of two generators:
- **Mission Graph**: generates a graph that consitute the game's mission.
- **Layout Map**: lays down the graph on a 2D grid of cells where each cell can be connected from any of the four main directions (North, South, East, and West).

### Mission Graph

The **Mission Graph** consists of 7 different type of nodes that can be found in the enum `ObstacleTowerGeneration.MissionGraph.NodeType`. Each node represent different game room:
- **Start**: is the room where the level starts.
- **End**: is the room where the level ends.
- **Key**: is a room that has to have a key in it.
- **Lock**: is a room that can only be reached through one door that is locked and require a key to open.
- **Lever**: is a room that connects rooms from behind a lock room to rooms that are before.
- **Puzzle**: is a room that involve a challenge that need to be passed to move on.
- **Normal**: is any other room other than the previous ones.

Every node in the **Mission Graph** have an access level. An access level is a number that determines how many **Puzzle** and **Lock** nodes you passed on before reaching that node. For example: a node with access level of 0 means that it is connected to the start node without having any puzzle or lock nodes in between, while a node with access level of 2 means that there is two nodes of type **Puzzle** or **Lock** in between the start node and that node. The access level is very helpful in generating the **Layout Map** and also for generating **Lever** nodes.

The **Mission Graph** uses graph grammar to generate the mission. All the grammar is implemented in `grammer/` folder. There is 8 different grammar rules:
- `addNormal`: adds a **Normal** node between two nodes of the same level.
- `addPuzzle`: adds a **Puzzle** node between two nodes of the same level which will raise the second node level.
- `addKeyLock`: adds a **Lock** node between two nodes of the same level which will raise the second node access level, then add a **Key** node as a branch from the first node.
- `addNormalToKey`: add a **Normal** node between two nodes of the same level where the second node is a **Key** node.
- `addLever`: connects a high access level node to a low access level node using a **Lever** node.
- `makeLink`: connects two nodes of the same access level using a normal link.
- `makeBranching`: change the structure of three consecutive nodes to make them as branching nodes.
- `moveLockBack`: moves a normal task from behind a **Lock** node towards before which will decrease its access level.

To adjust the generated

### Layout Map