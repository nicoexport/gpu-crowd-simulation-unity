using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

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

        [SerializeField] private int neighborActorCount = 3;


        [Header("Debug")]
        [SerializeField] private bool drawNeighbors = true;

        private Actor[] actors;

        List<Actor> neighborActors = new List<Actor>();
        List<float> neighborDistances = new List<float>();

        // DEBUG
        List<Actor> neighborActorsFirstActor = new List<Actor>();

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

            // calculating crowd center position
            for (int i = 0; i < actorCount; i++)
            {
                Actor currentActor = actors[i];
                DetermineNeighbors(i);
                
                currentActor.position = currentActor.position + currentActor.velocity * Time.deltaTime;
            }       
        }

        protected void OnDrawGizmos()
        {
            DebugDrawingUtility.DrawRect(spawnRect, Color.white);
            DrawActorGizmos();
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

            var currentActor = actors[currentIndex];    

            for (int i = 0; i < actorCount; i++)
            {
                // if the actor is the current one continue
                if (i == currentIndex)
                {
                    continue;
                }

                Actor otherActor = actors[i];
                float otherDistanceSquared = Vector2.Distance(otherActor.position, currentActor.position);

                int insertIndex = -1;

                // finding insertion index 
                for (int j = 0; j < neighborDistances.Count; j++)
                {
                    if (otherDistanceSquared < neighborDistances[j])
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
                        neighborDistances.Add(otherDistanceSquared);
                    }
                    continue;
                }

                // insertion index found
                neighborActors.Insert(insertIndex, otherActor);
                neighborDistances.Insert(insertIndex, otherDistanceSquared);

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
                Gizmos.color = Color.cyan;
                if (i == 0 && drawNeighbors)
                {
                    Gizmos.color = Color.orange;
                }

                Actor actor = actors[i];
                Vector2 position = actor.position;
                Vector2 velocity = actor.velocity;
                Gizmos.DrawSphere(position, 0.15f);
                DebugDrawingUtility.DrawArrowGizmo(position, velocity * 0.5f);
            }

            if (drawNeighbors)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < neighborActorsFirstActor.Count; i++)
                {
                    var neighbor = neighborActorsFirstActor[i];
                    Gizmos.DrawSphere(neighbor.position, 0.15f);
                }
            }
        }
    }
}