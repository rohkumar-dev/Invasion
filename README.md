# Invasion

### By Rohan Kumar

## Overview

**Invasion** is a third-person shooter where players must defend the last 25 civilians on Earth from an alien invasion. With a variety of weapons and strategic gameplay mechanics, players must fend off waves of aliens and protect humanity from extinction.

## Features

- **Wave-Based Combat**: Survive increasingly difficult waves of alien attackers.
- **Multiple Weapon Types**: Use an arsenal of weapons, including:
  - **Base Assault Rifle**: Medium-range, automatic fire.
  - **Golden Pistol**: High damage, low fire rate.
  - **Rocket Sniper**: Long-range, splash-damage rockets.
- **Dynamic AI Behavior**: Different alien types with unique abilities and behaviors.
- **Civilian AI**: Civilians react dynamically to threats and must be protected.
- **Replayability**: Procedurally spawned enemies and multiple strategies for success.

## High-Level Design

- **Type**: Third-Person Shooter  
- **Target Audience**: 10-40 years old  
- **Tone**: Goofy and light-hearted with cartoony visuals  
- **Theme**: "Protect by shooting"—Save civilians by exterminating alien threats.  

## Controls

- **Movement**: `WASD`  
- **Sprint**: `Shift` (limited duration)  
- **Jump**: `Spacebar`  
- **Reload**: `R`  
- **Aim and Shoot**: Mouse  

## Alien Types and AI Pathfinding

### 1. Player Attacker
- **Goal**: Disable the player’s guns and attack civilians while the player is defenseless.  
- **Pathfinding**: Targets the player first, then switches to civilians when the player is disabled.  
- **Behavior**: Updates targets dynamically based on proximity.

### 2. Bounty Hunter
- **Goal**: Charges at civilians at high speed with no ability to change targets.  
- **Pathfinding**: Locks onto one civilian and moves in a straight path, ignoring obstacles.  
- **Behavior**: Does not update target once locked.

### 3. Tank
- **Goal**: Attacks civilians and explodes upon death, causing splash damage.  
- **Pathfinding**: Targets the nearest civilian and updates target every few seconds.  
- **Behavior**: Prioritizes civilians in groups to maximize damage.

### 4. Hypnosis Alien
- **Goal**: Hypnotizes civilians, pulling them closer until captured.  
- **Pathfinding**: Locks onto the nearest civilian and stops moving once within range.  
- **Behavior**: Does not switch targets after locking in.

## Game Loop

1. Spawn with access to all weapons.
2. Defend civilians against waves of aliens.
3. Use vending machines to upgrade weapons.
4. Survive until all waves are cleared or civilians are lost.

## Art and Sound Design

- **Art Style**: Cartoony and light-hearted.
- **Animations**: Goofy, with comical movements for civilians and aliens.
- **Sound Design**: Intense background music during gameplay and humorous effects for civilians.

## Challenges

- **AI Pathfinding**: Balancing complexity with performance to handle large numbers of enemies.
- **Animation Rigging**: Creating intuitive and polished animations for alien attacks.

## Tools and Technologies Used

- **Engine**: Unity  
- **Assets**: Polygon Alien Pack and Farm Pack by Red Athena  
- **Audio Engine**: FMOD (planned integration)  
- **Pathfinding**: Unity NavMesh for AI behaviors  

## How to Play

1. Use `WASD` to move and `Shift` to sprint.
2. Shoot aliens before they reach civilians.
3. Upgrade weapons using vending machines.
4. Survive all waves to win the game.

## Known Issues

- Civilians may occasionally run toward aliens due to simplistic AI.
- Performance drops at higher wave counts may occur; optimization planned.

## Potential Add-ons and Future Improvements

- **Mobility Items**: Thrusters, jetpacks, and shields to enhance gameplay.  
- **Additional Maps**: More levels for variety and replayability.  
- **New Alien Types**: Advanced behaviors and abilities for added challenge.  
- **Utility Items**: Defensive tools like barriers and decoys.  
- **Multiplayer Support**: Cooperative and competitive modes.  
- **Gameplay Enhancements**: Expanded upgrade systems and weapon modifications.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
