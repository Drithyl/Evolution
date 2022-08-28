# Evolution
CS Final Project simulating the workings of evolution over multiple generations of creatures. Concept, terrain generation and custom terrain shader based on Sebastian Lague's excellent [ecosystem simulation](https://github.com/SebLague/Ecosystem-2/tree/master). Most of the borrowed code has been refactored or rebuilt from the grounds up.

More details on the goals of the project can be found in my [midterm report](https://docs.google.com/document/d/1kLYXM7bCpaHHlvhtzHdlWk9fkTlYQ4Y3WisZ4CgeYk8/edit?usp=sharing).


## Current Progress

* Procedural terrain generation through Unity's Perlin Noise and programmatic mesh creation.
* Two species of creatures (male/female) who grow old, feed, drink and reproduce. The herbivores consume plant food that grows over time throughout the world, and the predators consume the herbivores.
* Series of Gene components with randomized values that are passed down to children by splicing the father and mother's genome in random ratios.
* Behaviour of creatures is determined by urge levels, some of which are genetic-related, like the urge to seek mates.
* Individual and global statistics for better feedback: tiles walked, months aged, food and water consumed, causes of death, etc.
* Working line graphs as 3d objects in the world to display feedback (population, causes of death, average nutrition gene values and average reproduction gene values).
* FoodType enum to support different diets (herbivore and carnivore) through the same HungerGene without modification.
* Easy filtering and options in methods. TerrainTypes are a bitmask so tiles can have multiple types within one value; Sex is now an enum like FoodType.
* Values have been tweaked to the point where the herbivore species can survive indefinitely, with ups and downs in population.
* Working in-game 3rd person camera to fly through the world and observe the simulation or follow specific creatures.


## Next Steps

* Creating even more graphs to show statistics.
* Tweak values of predators and herbivores to achieve an interesting evolution simulation.
* Optimize pathing and other areas to enable for larger populations and faster simulations.
* Use proper models to prettify things.
