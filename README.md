For my final project, I made a game called `/re/`. It’s a simple 2D physics-based platformer where the artifacts of your previous runs persist between checkpoints. The objective is to get to the end of the single level, but every death leaves a corpse in the world. You’ll have to persist between lives to build off your past accomplishments to be able to finish the game. The scoring model is based off your past lives, so try to be as efficient as possible. 

You can play it [here](http://adriangarza.github.io/re/), and be sure to check out the full source code [here](https://github.com/adriangarza/re/tree/master/Assets/Scripts). You can see the timeline of the game’s development through [these commit messages](https://github.com/adriangarza/re/commits/master).

## Programming
One interesting problem I encountered during development was dealing with past lives - how exactly to treat death and keep track of the remaining player corpses in the level. If I left too many of them in the game it would slow everything down, but too few would make the game less interesting.

So I created a “graveyard” object to store the corpses - nothing visually represented in the game, but a hierarchical structure of organization. Whenever the player hits a strategically placed checkpoint, the graveyard will be [cleared](https://github.com/adriangarza/re/blob/master/Assets/Scripts/PlayerController.cs#L197) and the corpses will disappear, freeing up memory and CPU resources from simulating dozens of squares. 

The death itself was also an interesting thing to code. I was working simultaneously with two separate metaphors: the game for the cycle of rebirth and the code for manipulations of pixels. There’s a great [article](http://www.ctheory.net/articles.aspx?id=74) that goes more into detail about the latter, but that’s the only part applicable to this course. 

Regardless, when the player hits a hazard, the [`Die()`](https://github.com/adriangarza/re/blob/master/Assets/Scripts/PlayerController.cs#L162) method is called. The player’s last position, movement vector, and angular velocity are retained in memory and they’re teleported back to the last checkpoint. A pre-made corpse object is then placed at the player’s previous location and given the same speed and rotation. If this didn’t all happen near-instantaneously at the hardware level, there might be some interesting metaphors for the bardo, but as it stands now death and rebirth happen very quickly, and so the bardo lasts for all of a millisecond.

## Physics
Another challenge was movement. I wanted the game to have a simplistic visual style, yet feel like the player moving something bouncy and alive instead of a rigid square. There was also the issue of past corpses - the realism of their behavior depended on their ability to rotate, and letting the corpses rotate but not the player gave the game a sense of inconsistency. To solve this, I applied a small rotational force in the direction the player was moving, so their square would bounce and roll along the ground instead of sliding. It immediately establishes the language of the game as a physics-based platformer. 

However, that tumbling movement broke the jump ability. I only let the player jump when they were in contact with the ground, and since the contact was continuously being broken, you’d have to be frame-perfect for jump timing. So instead of immediately revoking jump privileges when they broke ground contact, I left the player with a [0.2s window](https://github.com/adriangarza/re/blob/master/Assets/Scripts/PlayerController.cs#L148) in which to jump. This actually let the player jump for a fraction of a second after they fell off a ledge, but it wasn’t game-breaking so I let it stay.

After coding the game’s architecture, I actually ended up spending the most time on level design. Like I mentioned with the physics, I needed to set the tone of the game early on - make death an immediately necessary aspect, but make the player realize they needed to minimize the total death count. 

## Visuals
The game’s colors went through several iterations, but I settled on a neutral blue base with gold highlights and red/pink primary colors to distinguish the player. I knew I wanted to keep things simple for the player, but for the other assets like the background art and all the hazards, I had a choice between pixel-painting with a massive range of colors or restricting myself to a few colors and using a lot of [dithering](https://en.wikipedia.org/wiki/Dither) (pixel-perfect patterns that create the illusion of a gradient). 

In the end, I settled on dithering because the reduced color palette gave the game a more unified look. It was a bit of a challenge to keep the background and clouds from getting too muddy, but I think I achieved a good balance of large color fields for the illusion of highlights and shadows.

To add to the illusion of depth, the background elements move at different speeds relative to the player: the background image barely scrolls at all and the clouds move only slightly faster. I achieved this with a simple [parallax script](https://github.com/adriangarza/re/blob/master/Assets/Scripts/Parallax.cs).

## Gameplay
I struggled for a while with how to represent the different types of hazards. I knew I wanted something that would kill the player but leave their corpse in the level, and something else that would destroy both the player and their corpse. I settled on electricity for the former. I originally planned the latter to be fire, but since fire is hard to animate, I settled for spinning saws that immediately convey a new type of hazard, and carry the connotations of a disappearing corpse to people who know them from past video games.

I also wanted visual feedback on death. It was a little jarring to have the camera instantly snap back to the player’s respawn point, so I went with the classic metaphor of white light and made the screen flash white for a few frames on death to signify some kind of broken continuity.	
