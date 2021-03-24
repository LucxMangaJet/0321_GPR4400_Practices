using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVector
{
    struct Vector
    {
        public float x;
        public float y;
        public float z;

        public float magnitude { get => GetMagnitude(); }

        public Vector normalized { get => GetNormalized(); }

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float GetMagnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public Vector GetNormalized()
        {
            return this * (1 / magnitude);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector operator *(Vector v, float s)
        {
            return new Vector(v.x * s, v.y * s, v.z * s);
        }

        public static Vector operator *(float s, Vector v)
        {
            return v * s;
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public override string ToString()
        {
            return $"Vector {{ {x}, {y}, {z}}}";
        }

        public static float Dot(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector Cross(Vector a, Vector b)
        {
            return new Vector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public static float AngleBetween(Vector v1, Vector v2)
        {
            return (float)(Math.Acos(Dot(v1, v2) / (v1.magnitude * v2.magnitude)) * (180 / Math.PI));
        }
    }
}