# DungeonMind
### Procedural Dungeon Evaluation using AI Simulation

DungeonMind is a Unity based system that generates procedural dungeon layouts and evaluates their quality using automated AI agents. The project explores how artificial intelligence can assist game developers in analyzing level design difficulty, structure, and exploration efficiency requiring less manual playtesting.

---

# Project Goals

The main goal of this project is to use AI agents to:

* validate solvability of procedurally generated levels
* measure exploration difficulty
* identify inefficient or confusing layouts
* provide useful metrics to guide level design decisions

The system generates dungeon layouts, simulates agents navigating them, and records performance data that can be analyzed to evaluate quality.

---

# Core Features

## Procedural Dungeon Generator

Creates grid based dungeon layouts consisting of:

* randomized room placement
* corridor connections between rooms
* optional branching paths
* start and exit positions
* configurable map size and room parameters

The generator produces varied layouts that range from simple paths to complex branching structures.

---

## AI Simulation Agents

### Perfect Solver (A*)

An optimal pathfinding agent that always finds the shortest path from the start to the exit. This agent establishes a baseline measurement for minimum traversal cost.

Tracked metrics include:

* optimal path length
* completion time
* tiles visited

---

### Fog-of-War Explorer

An exploration based agent that navigates the dungeon using limited knowledge of the environment. The agent reveals tiles gradually and must backtrack when encountering dead ends.

Tracked metrics include:

* tiles discovered
* tiles visited
* total steps taken
* number of backtracks
* completion time
* efficiency ratio compared to optimal path

---

## Dungeon Complexity Scoring

Each generated dungeon is analyzed using a heuristic complexity score based on structural characteristics such as:

* number of rooms
* number of walkable tiles
* branching points
* dead ends
* optimal path length

The complexity score provides a quantitative estimate of how challenging a dungeon may be to explore.

---

## Simulation Data Collection

Simulation results are stored in JSON format and include:

* agent performance metrics
* dungeon structural metrics
* complexity score
* completion statistics

These datasets allow for comparison across multiple runs and support further analysis or machine learning experiments.

---

# Technologies Used

* Unity (2D URP)
* C#
* A* pathfinding
* procedural generation algorithms
* JSON data serialization

---

# Python Requirements for ML

This project requires python to be installed (And in your PATH variable)
* [Python 3.14 Installer](https://www.python.org/ftp/python/3.14.4/python-3.14.4-amd64.exe)

Additionally, run the following command post install
* `pip install pandas scikit-learn`

# Example Metrics

Example metrics collected from simulation runs include:

* steps taken
* backtrack count
* completion time
* tiles discovered
* tiles visited
* efficiency ratio
* dungeon complexity score

These values allow comparisons between different dungeon layouts and agent behaviors.

---

# Author

[Khris Boter](https://github.com/khrisboterdev)