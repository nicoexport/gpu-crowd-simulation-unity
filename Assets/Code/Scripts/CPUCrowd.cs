using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crowds
{
    public class Actor
    {
        public Vector2 position;
        public Vector2 velocity;

        public Actor(Vector2 position, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }
    }

    public class CPUCrowd : MonoBehaviour
    {
        [SerializeField] private int actorCount = 10;
        [SerializeField] private Rect spawnRect = new();

        [SerializeField] private float centeringStrength = 0.3f;
        [SerializeField] private float globalTargetStrenth = 0.67f;
        [SerializeField] private float velocityMatchingStrength = 0.5f;
        [SerializeField] private float collisionAvoidanceStrength = 0.5f;
        [SerializeField] private float collisionAvoidanceRadius = 0.5f;

        [SerializeField] private int neighborActorCount = 3;

        [Header("Target")]
        [SerializeField] private Transform globalTargetTransform;

        private Actor[] actors;

        private List<Actor> neighborActors = new();
        private List<float> neighborDistances = new();

        // DEBUG
        [Header("Debug")]
        [SerializeField] private bool drawNeighbors = true;
        private List<Actor> neighborActorsFirstActor = new();
        private Vector2 localCenterPositionFirstActor = Vector2.zero;

        protected void Start()
        {
            SpawnActors();
        }

        protected void FixedUpdate()
        {
            // update actors velocity
            // steps: 
            // collision avoidance
            // velocity matching
            // crowd centering

            for (int i = 0; i < actorCount; i++)
            {
                Actor currentActor = actors[i];
                DetermineNeighbors(i);


                // local crowd centering
                Vector2 positionAccumulator = Vector2.zero;
                Vector2 velocityAccumulator = Vector2.zero;


                positionAccumulator += currentActor.position;
                for (int j = 0; j < neighborActorCount; j++)
                {
                    Actor neighbor = neighborActors[j];
                    positionAccumulator += neighbor.position;
                    velocityAccumulator += neighbor.velocity;
                }

                Vector2 localCenterPosition = positionAccumulator / (neighborActorCount + 1);
                Vector2 vectorToLocalCenter = localCenterPosition - currentActor.position;

                Vector2 localAverageVelocity = velocityAccumulator / (neighborActorCount + 1);

                if (drawNeighbors && i == 0)
                {
                    localCenterPositionFirstActor = localCenterPosition;
                }

                // currentActor.velocity = Vector2.Lerp(currentActor.velocity, localVelocity, velocityMatchingLerpFaktor);


                // acceleration:  m/s^2  velocity: m/s  

                // 1. --- collision avoidance ---
                // for  now simple neighbor avoidance.
                // if the distance between current actor and its neighbor is smaller then a given collision threshold calculate a velocity away from the neighbor
                // to improve this a quick google search resulted in either Optimal Reciprocal Collision Avoidance (ORCA) or Social Force Model (Repulsive Forces)
                Vector2 collisionAvoidanceAccumulator = Vector2.zero;
                int collisionAvoidanceAccumulatorCount = 0;
                for (int j = 0; j < neighborActorCount; j++)
                {
                    Actor neighbor = neighborActors[j];
                    float distanceToNeighbor = Vector2.Distance(currentActor.position, neighbor.position);
                    if (distanceToNeighbor < collisionAvoidanceRadius)
                    {
                        collisionAvoidanceAccumulatorCount++;
                        Vector2 force = (currentActor.position - neighbor.position).normalized * (collisionAvoidanceRadius - distanceToNeighbor);
                        collisionAvoidanceAccumulator += force;
                    }
                }
                if (collisionAvoidanceAccumulatorCount > 0)
                {
                    Vector2 averageCollisionAvoidanceVector = collisionAvoidanceAccumulator / collisionAvoidanceAccumulatorCount;
                    currentActor.velocity += averageCollisionAvoidanceVector * Time.deltaTime * collisionAvoidanceStrength;
                }

                // --- 2. global target following ---
                if (globalTargetTransform)
                {
                    Vector2 globalTargetPosition = globalTargetTransform.position;
                    Vector2 directionToGlobalTarget = (globalTargetPosition - currentActor.position).normalized;
                    currentActor.velocity += directionToGlobalTarget * Time.deltaTime * globalTargetStrenth;
                }
                // --- 3. velocity matching ---
                Vector2 velocityMathchingSteerAcceleration = (localAverageVelocity - currentActor.velocity) * velocityMatchingStrength;
                currentActor.velocity += velocityMathchingSteerAcceleration * Time.deltaTime;
                // --- 4. local centering ---
                currentActor.velocity += vectorToLocalCenter * Time.deltaTime * centeringStrength;

                // update position based on velocity
                currentActor.position += currentActor.velocity * Time.deltaTime;
            }
        }

        protected void OnDrawGizmos()
        {
            //DebugDrawingUtility.DrawRect(spawnRect, Color.white);
            DrawActorGizmos();

            if (globalTargetTransform)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(globalTargetTransform.position, 0.25f);
            }
        }

        private void SpawnActors()
        {
            actors = new Actor[actorCount];

            for (int i = 0; i < actorCount; i++)
            {
                // random spawn position in rect
                float positionX = Random.Range(spawnRect.xMin, spawnRect.xMax);
                float positionY = Random.Range(spawnRect.yMin, spawnRect.yMax);
                Vector2 position = new(positionX, positionY);

                float dirX = Random.Range(-1.0f, 1.0f);
                float dirY = Random.Range(-1.0f, 1.0f);
                Vector2 direction = new Vector2(dirX, dirY).normalized;
                float velocityMagnitude = Random.Range(0.0f, 1.0f);

                Vector2 velocity = direction * velocityMagnitude;

                actors[i] = new Actor(position, velocity);
            }
        }

        private void DetermineNeighbors(int currentIndex)
        {
            // determine neigbor actors 
            neighborActors.Clear();
            neighborDistances.Clear();

            Actor currentActor = actors[currentIndex];

            for (int i = 0; i < actorCount; i++)
            {
                // if the actor is the current one continue
                if (i == currentIndex)
                {
                    continue;
                }

                Actor otherActor = actors[i];
                float otherDistance = Vector2.Distance(otherActor.position, currentActor.position);

                int insertIndex = -1;

                // finding insertion index 
                for (int j = 0; j < neighborDistances.Count; j++)
                {
                    if (otherDistance < neighborDistances[j])
                    {
                        insertIndex = j;
                        break;
                    }
                }

                // insert if no spot was found, but list is not "full" (neighborCount)
                if (insertIndex == -1)
                {
                    if (neighborActors.Count < neighborActorCount)
                    {
                        neighborActors.Add(otherActor);
                        neighborDistances.Add(otherDistance);
                    }
                    continue;
                }

                // insertion index found
                neighborActors.Insert(insertIndex, otherActor);
                neighborDistances.Insert(insertIndex, otherDistance);

                // trim to max size
                if (neighborActors.Count > neighborActorCount)
                {
                    neighborActors.RemoveAt(neighborActors.Count - 1);
                    neighborDistances.RemoveAt(neighborDistances.Count - 1);
                }
            }

            // Add first actors neighbors to the debug list for visualization
            if (drawNeighbors && currentIndex == 0)
            {
                neighborActorsFirstActor = neighborActors.ToList();
            }
        }

        private void DrawActorGizmos()
        {
            if (actors == null)
            {
                return;
            }

            for (int i = 0; i < actorCount; i++)
            {
                Actor actor = actors[i];
                Gizmos.color = Color.cyan;

                if (i == 0 && drawNeighbors)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(actor.position, collisionAvoidanceRadius);

                    Gizmos.color = Color.orange;
                }

                Vector2 position = actor.position;
                _ = actor.velocity;
                Gizmos.DrawSphere(position, 0.15f);
                //DebugDrawingUtility.DrawArrowGizmo(position, velocity * 0.5f);
            }

            if (drawNeighbors)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < neighborActorsFirstActor.Count; i++)
                {
                    Actor neighbor = neighborActorsFirstActor[i];
                    Gizmos.DrawSphere(neighbor.position, 0.15f);
                }

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(localCenterPositionFirstActor, 0.15f);
            }
        }
    }
}