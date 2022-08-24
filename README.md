# Evolution
CS Final Project simulating the workings of evolution over multiple generations of creatures. Concept, terrain generation and custom terrain shader based on Sebastian Lague's excellent [ecosystem simulation](https://github.com/SebLague/Ecosystem-2/tree/master). Most of the borrowed code has been refactored or rebuilt from the grounds up.

More details on the goals of the project can be found in my [midterm report](https://docs.google.com/document/d/1kLYXM7bCpaHHlvhtzHdlWk9fkTlYQ4Y3WisZ4CgeYk8/edit?usp=sharing).


## Current Progress

* Procedural terrain generation through Unity's Perlin Noise and programmatic mesh creation.
* One species of creatures (male/female) who grow old, feed, drink and reproduce. Food in the world gets consumed and grows back.
* Series of Gene components with randomized values that are passed down to children by splicing the father and mother's genome in random ratios.
* Behaviour of creatures is determined by urge levels, some of which are genetic-related, like the urge to seek mates.
* Individual and global statistics for better feedback: tiles walked, months aged, food and water consumed, causes of death, etc.
* Working line graphs as 3d objects in the world to display feedback. At the moment, only population line graph is active.


## Next Steps

* Creating more working graphs to display other statistics like average values of genes.
* New carnivore species that prey on the herbivores to add a more active threat and means of population control.
