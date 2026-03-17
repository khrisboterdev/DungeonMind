# Architecture

## Systems

### Dungeon Generator
- Generates grid-based dungeon
- Outputs tile map + metadata

### AI Agent
- Receives dungeon state
- Decides actions (move, attack, pickup)

### Simulation Manager
- Runs multiple playthroughs
- Resets environment

### Metrics System
- Tracks:
  - completion rate
  - time to exit
  - damage taken

## Data Flow

Dungeon → AI Agent → Simulation → Metrics → Results