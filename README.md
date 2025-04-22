# Team Unity Game Jam

**Game Title**: RealmBlazers\
**Team Members**: Lyndrinn Yao (lhy2), Connie Xu (cx71), Emily Sun (ejs83), Ashton Caldwell (arc100)\
**GitHub Link**: https://github.com/sylvia5monthes/390_realmblazers

## Game Pitch
RealmBlazers is a tactical J-RPG where you command uniquely skilled units on a grid-based battlefield. Using positioning and abilities to overcome enemies across different levels, reunite your scattered party and escape the shadow realms to defeat the Demon Lord.

## How to Play
This game only requires a laptop with a trackpad or a mouse. This game is played mostly using the mouse/trackpad functionalities, where left-click is used for most selections and right click is used for selecting grid units to place units down. 

To play the game, players should play starting from the StartMenu scene.

For each level, you have a series of playable units that can be placed in an initial area. Once you place all the units down, the actual gameplay begins starting with the player phase. For each unit that you have alive, you can select their sprites on the grid to view their action menus, which includes their stats, current health, and move pool. You can choose to either move and then perform an action, only perform an action, or do nothing by waiting. Pressing the Escape key cancels a pending move or action if you've selected one but decide not to perform it. Player movement and attack results are determined by their class stats, which are coded into the game. You can also select enemy units to view their current health, stats, and attacks. After each player has finished their turn (movement and action) and the enemies have finished their counterattacks, then the enemy phase begins. Each enemy can also choose to move and act depending on Enemy Logic AI, and player units that have been attacked can counterattack.

To break down some of the action mechanics, each unit can attack any enemies in range. The damage is calculated in the backend of the game and the corresponding results are displayed on the screen. Units that were attacked, if they are still alive, have the opportunity to retaliate against the original attack. This damage is also calculated on the backend and then displayed on the screen.

When the player finishes killing all the minions in one stage, a boss enemy unit will spawn in and re-trigger the player phase. This boss has higher stats and stronger attacks than the minion enemy unit, so players will need to strategically control their units to defeat the enemies.

There are also some terrain effects on each stage. Brush increases the chance that the unit inside will dodge an attack. Magma in stage two will decrease the unit's defense. Obstacles prevent units from standing on certain squares.

Press P to pause and unpause the game. Click to progress dialogue/cutscenes.

## Content Amount and Type
There are three levels that are available to play. Each level contains a small legion of enemies, that once defeated, release the boss enemy. Upon defeating the boss enemy, dialogue will play, and the next stage will load. After the third stage, a victory screen is displayed, and the player can return to the main menu, signifying the end of the RealmBlazers demo.

It is important to start from the StartMenu scene since pressing Begin from this scene will play a cutscene. Audio should be played as well since all audio tracks were originally designed by Lyndrinn Yao. We included a level select in the main menu in case a level is too difficult, thus allowing the player to skip ahead if needed. Preferably however, the game should be played from beginning to end in order to see all the cutscenes and dialogues.

To experience the entirety of RealmBlazers, it is expected that the player experiments with the terrains available and the abilities of each unit provided. However, if the player plays from beginning to end, starting from the StartMenu scene and ending with the Victory scene, it is likely that the player will experience everything in RealmBlazers.

## Lessons Learned
One of the biggest lessons that Shenanigames has learned from this project was the importance of the scope issue. With so many ideas and mechanics of interest, we ran into issues regarding a lack of time and overcomplicated code. We learned to keep the objective as well as the mechanics succinct while attempting to ramp up the challenge with each stage. 

Additionally, we learned how imperative it is to assign roles to each team member. Since each member of Shenanigames worked on whatever seemed important, merge conflicts became rampant and divided tasks became disorganized at times. Through perseverance and teamwork however, we managed to come together and ensure that the final product was functional, and that it captured the vision we originally wished for RealmBlazers.

Finally, we discovered a keen enjoyment for creating an entertaining product from scratch. Shenanigames is proud to have worked hard together to create a product that we are all satisfied to call our debut title. RealmBlazers was able to represent our vision for an enjoyable game, and we are very glad to have had the chance to work on it together!
