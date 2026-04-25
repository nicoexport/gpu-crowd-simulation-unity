# gpu-crowd-simulation-unity
Simulating crowds based on Craig W. Reynolds (1987) flocking simulation.

# Description
This is a work in process project exploring crowd, flock, herd or school simulations as a form of particle simulation in Unity based on the flocking simulation presented by Craig W. Reinolds (1987) in "Flocks, Herds, and Schools: A Distributed Behavioral Model" ([source](https://dl.acm.org/doi/10.1145/37402.37406)).

The goal is to simulate crowds of people moving through confined spaces. To make this scalable to large entity counts, this projects aims to make use of Unitys compute shader support leveraging the massively parallel processing power of modern GPUs.

# Flocking
According to Reynolds natural flocks consist of 2 balanced but contrary desires:
- staying close to the (center of) the flock
- avoiding collision with other entities

In huge flocks a single actor can not keep track of all other actors, it has a localized filtered perception of the world:
- itself
- two or three nearest neighbors
- rest of the flock

## Actors
A actor can be any entity in a flock, herd, school or crowd and is defined by a small set of data points and some behavior.

```
struct Actor {
  Vector2 position;
  Vector2 velocity;
}
```
The most bare bones data each actor has is its position and its current velocity. Each iteration the system then moves each actor according to its current velocity and updates its position.

## Simulated Flocks
In order to simulate the influence of the flock on the movement of each single actor the flocking system calculates the resulting velocity for each new iteration.
The following behaviors, corresponding to the urges of avoiding collisions and joining the flock, are considered in order of priority:

1. Collision avoidance
2. Velocity Matching (with nearby flockmates)
3. Flock centering

These behaviors simulate the influence of the flock on the movement of the actor, while it is possible to add more behaviours to the accumulation, like a global target position the flock tries to reach.

## Accumulation of velocities 
Naively applying the calculated velocities of each behavioral urge may lead to unrelaistic behavior. An example for this is the velocity mathing and centering urge overpowering collision avoidance with obstacles in the environment or eachother resulting in members of the flock colliding at high speeds. According to Reynolds this can be solved in various ways, while *Priorized acceleration allocation* is easy and straight forward.

This is based on a strict ordering of all component behaviours. The acceleration requests are considered in priority order and added to an accumulator, while the magnitude of these requests are added to another accumulator. This continues until the sum of accumulated magnitudes gets larger then a *maximum acceleration* value (defined per boid). The last contributing behaviours magnitude gets trimmed, so it doesnt surpass this maximum. 

This ensures that the most relevant urges are satisfied first while lower priority ones, like the flock centering urge can be skipped temporarily.


# Sources 
Craig W. Reynolds. 1987. Flocks, herds and schools: A distributed behavioral model. SIGGRAPH Comput. Graph. 21, 4 (July 1987), 25–34. [doi](https://doi.org/10.1145/37402.37406)
