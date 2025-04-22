# Team Unity Game Jam

**Game Title**: RealmBlazers\
**Team Members**: Lyndrinn Yao (lhy2), Connie Xu (cx71), Emily Sun (ejs83), Ashton Caldwell (arc100)\
**GitHub Link**: https://github.com/sylvia5monthes/390_realmblazers

## Game Pitch
RealmBlazers is a tactical J-RPG where you command uniquely skilled units on a grid-based battlefield. Using positioning and abilities to overcome enemies across different levels, reunite your scattered party and escape the shadow realms to defeat the Demon Lord.

## How to Play
This game only requires a laptop with a trackpad or a mouse. This game is played exclusively using the mouse/trackpad functionalities, where left-click is used for most selections and right click is used for selecting grid units to place units down. 

To play the game, players should play starting from the StartMenu scene.

For each level, you have a series of playable units that can be placed in an initial area. Once you place all the units down, the actual gameplay begins starting with the player phase. For each unit that you have alive, you can select their sprites on the grid to view their action menus, which includes their stats, current health, and move pool. You can choose to either move and then perform an action, only perform an action, or do nothing by waiting. Player movement and attack results are determined by their class stats, which are coded into the game. You can also select enemy units to view their current health, stats, and attacks. After each player has finished their turn (movement and action) and the enemies have finished their counterattacks, then the enemy phase begins. Each enemy can also choose to move and act depending on Enemy Logic AI, and player units that have been attacked can counterattack.

To break down some of the action mechanics, each unit can attack any enemies in range. The damage is calculated in the backend of the game and the corresponding results are displayed on the screen. Units that were attacked, if they are still alive, have the opportunity to retaliate against the original attack. This damage is also calculated on the backend and then displayed on the screen.

When the player finishes killing all the minions in one stage, a boss enemy unit will spawn in and re-trigger the player phase. This boss has higher stats and stronger attacks than the minion enemy unit, so players will need to strategically control their units to defeat the enemies.

## Content Amount and Type


## Lessons Learned

