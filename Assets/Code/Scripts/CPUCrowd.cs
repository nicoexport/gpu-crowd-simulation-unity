using UnityEngine;
using Utility;

namespace Crowds
{
    public struct Actor
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

        private Actor[] actors;

        protected void Start()
        {
            SpawnActors();
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

                Vector2 velocity = Vector2.up;

                actors[i] = new Actor(position, velocity);
            }
        }

        private void DrawActorGizmos()
        {
            if (actors == null)
            {
                return;
            }

            Gizmos.color = Color.orange;
            for (int i = 0; i < actorCount; i++)
            {
                Actor actor = actors[i];
                Vector2 position = actor.position;
                Vector2 velocity = actor.velocity;
                Gizmos.DrawWireSphere(position, 0.25f);
                DebugDrawingUtility.DrawArrowGizmo(position, velocity * 0.5f);
            }
        }
    }
}