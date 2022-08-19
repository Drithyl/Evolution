# Evolution
CS Final Project simulating the workings of evolution over multiple generations of creatures. Concept, terrain generation and custom terrain shader based on Sebastian Lague's excellent [ecosystem simulation](https://github.com/SebLague/Ecosystem-2/tree/master). Most of the borrowed code has been refactored or rebuilt from the grounds up.


## Current Progress

### Basic functionality in place:

* Procedural terrain generation through Unity's Perlin Noise and programmatic mesh creation.
* One species of creatures (male/female) who grow old, feed, drink and reproduce. Food in the world gets consumed and grows back.
* Series of Gene components with randomized values that are passed down to children by splicing the father and mother's genome in random ratios.
* Individual and global statistics for better feedback: tiles walked, months aged, food and water consumed, causes of death, etc.


## Next Steps

* Gene mutation to each new generation to simulate actual evolution, rather than just inheritance of parent values.
* New carnivore species that prey on the herbivores to add a more active threat and means of population control.
