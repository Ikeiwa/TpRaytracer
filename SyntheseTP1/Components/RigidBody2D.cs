using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Components
{
    class RigidBody2D : Component
    {
        public Vector3 velocity;
        public float dragCoef = 0.37f;
        public float mass = 10.0f;
        public Vector3 gravity = new Vector3(0, 9.81f, 0);

        public RigidBody2D(GameObject owner) : base(owner){}

        public override void Update()
        {
            Vector3 friction = -(dragCoef * velocity.Length() * velocity / mass);
            velocity += (gravity + friction) * Time.deltaTime;

            Owner.position += velocity * Time.deltaTime;
        }

        public void AddForce(Vector3 force)
        {
            velocity += (force / mass)*Time.deltaTime;
        }
    }
}
