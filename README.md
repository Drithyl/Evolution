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
* FoodType enum to support different diets (herbivore and carnivore) through the same HungerGene without modification. Will allow easy addition of new carnivore species
* Refactored to enable easier filtering and options in methods. TerrainTypes are a bitmask so tiles can have multiple types within one value; Sex is now an enum like FoodType.
* Values have been tweaked to the point where the current species can survive indefinitely, with ups and downs in population.
* Added predators that devour the herbivores and reproduce just like them.
* Added line graphs for causes of death per month and average values of nutrition genes (hunger and thirst).


## Next Steps

* Creating even more graphs to show statistics.
* Tweak values of predators and herbivores to achieve an interesting evolution simulation.
* Optimize pathing and other areas to enable for larger populations and faster simulations.
* Use proper models to prettify things.
* Ingame cameras to follow specific creatures and look at graphs.
