# Void-Champions---My-Scripts
Code that I developed for the game Void Champions (https://miguelseabra1999.itch.io/voidchampions)

These files belong to the Void Champions project.
Some of the objects are attributed through Unity engine.


- Connecting.cs: a script to change images on display to make it look like the TV is glitching,
 while waiting for connection to establish. 
(nothing special, but we thought it would be an interesting touch)

- DillemaMenu.cs: handles a menu that is triggered once entering a dillema room. Player has to choose
 from "Share" or "Fight".

- GamepadUIController.cs: Unity base UI objects have weird limitations and having UI that supports both Controller
and Keyboard+Mouse is not their strength. So, I made the UI for Keyboard+Mouse and then made this script to add Controller
support (also turns on a shadow like object to know which element of UI we are hovering at the moment).

- HoverHPBar.cs: handles an HP Bar that hovers on top of each player and npc.

- InGameMenu.cs: handles the menu reachable by pressing Esc(or Select) while in a game/match.

- Leaderboard.cs: is not really a leaderboard, it just shows the players that is currently in first place. We wanted to
keep this ominous to create tension, but we realized, through playtests, that it was way too confusing and knowing who's
the first, helps the others to realize which battles are worth fighting and forming alliances to defeat that 1st player.
(small UI in top right corner)

- MetersHUD.cs: handles the main HP and Mana bar as well as the menu triggered by the cog. I realise that last menu got
too hard to read due to it's potential lenght, should be rethough in the future, maybe stack together power ups that are
of the same type and just show the total value, to get it cleaner.

- PowerUpMenu.cs: handles the UI menu that deals with choosing power ups. Chooses 3 power ups out of all possible power ups,
then the player chooses and it applies the chosen boost to the player. Power Ups have tiers so that players get better
power ups throughtout the levels. Boss power ups are for the player that is about to win and fight the other 3 at once,
so that he gets stronger power ups and balances the odds of a 1v3 fight.

- VictoryScreen.cs: Handles the UI that appears when the game ends, revealing players and their stats.

- Attributes.cs: players' attributes with functions triggered by the power up menu to buff the attributes.

- Stats.cs: registers stats that are displayed on MetersHUD or on the VictoryScreen, for players to keep track of how
successful they are being on that game (same idea of the Rainbow Six Siege scoreboard).

- VictoryNetwork.cs: only clients store their own Stats, but we display the stats of all players on the Victory Screen,
so we need to share that data and so VictoryNetwork is an interface that deals with network "logistics".
