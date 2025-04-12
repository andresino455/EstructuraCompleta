using OpenTK.Mathematics;
using System;

namespace ProgramacionGrafica.Principal
{
    public class Camara
    {
        public Vector3 Position { get; set; }
        public float Pitch { get; set; } = 0.0f; // Rotación vertical
        public float Yaw { get; set; } = -90.0f; // Rotación horizontal

        private float _fov = 45f;
        private float _sensitivity = 0.1f;
        private float _speed = 2.5f;

        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;

        public Camara(Vector3 posicion, Vector3 objetivo, Vector3 up)
        {
            Position = posicion;
            Front = Vector3.Normalize(objetivo - posicion);
            Up = up;
            Right = Vector3.Normalize(Vector3.Cross(Front, Up));
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public void ProcessKeyboard(Vector2 direction, float vertical, float deltaTime)
        {
            float velocity = _speed * deltaTime;

            if (direction.Y > 0)
                Position += Front * velocity;
            if (direction.Y < 0)
                Position -= Front * velocity;
            if (direction.X > 0)
                Position += Right * velocity;
            if (direction.X < 0)
                Position -= Right * velocity;
            if (vertical > 0)
                Position += Up * velocity;
            if (vertical < 0)
                Position -= Up * velocity;
        }


        public void ProcessMouse(float deltaX, float deltaY)
        {
            Yaw += deltaX * _sensitivity;
            Pitch -= deltaY * _sensitivity;

            Pitch = MathHelper.Clamp(Pitch, -89f, 89f);

            UpdateVectors();
        }

        private void UpdateVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            Front = Vector3.Normalize(front);
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}
