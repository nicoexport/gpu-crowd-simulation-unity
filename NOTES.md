# NOTES

## Implementation Considerations

### Local Perception

Each actor only has a local perception of "x" nearest neighbors. 
How do we get access to the neares neighbors?

What knowledge do we need from the neighbors: 

- average velocty (velocity matching)
- average position (centering) 
- position for collision avoidance

#### Naive Approach (SLOW)

- Keep a collection of the "x" nearest neighbors. 
- Loop over all neighbors and update the collection. 

Complexity: n * n * x ? (n = total number of actors)

#### Spatial Data Structures (FASTER)

Running a preprocess step that sorts the actors into a spatial data structure makes this way faster. A option that is currently considered for this is a Quad-Tree for two dimensions and a Oct-Tree for three dimensions.  

##### Quad- / Oct-Tree


## DEBUG VISUAL IDEAS

### Single Actor Highlighting

Goal: Visualize competing acceleration reqests.

Since one actor has a local perception of its x closest neighbors it could be useful to visually highlight a single actor and its different acceleration requests:

- saturate highlighted actor
- saturate percieved neighbors
- mute the colors of all other actors
- center and zoom camera on the highlighted actors