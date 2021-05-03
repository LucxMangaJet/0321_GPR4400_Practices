using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomMath
{
    public struct Vector 
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float Magnitude;
        public float SqrMagnitude;
        
        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            Magnitude = Mathf.Sqrt(X * X + Y * Y + Z * Z);
            SqrMagnitude = X * X + Y * Y + Z * Z;
        }
        public static Vector GetDistance(Vector a, Vector b)
        {
            Vector vector = b - a;
            return vector;
        }
        public static Vector operator - (Vector a, Vector b)
        {
            Vector vector = new Vector();
            vector.X = a.X - b.X;
            vector.Y = a.Y - b.Y;
            vector.Z = a.Z - b.Z;
            return vector;
        }
        public static Vector operator * (Vector a, float f)
        {
            Vector vector = new Vector();
            vector.X = a.X * f;
            vector.Y = a.Y * f;
            vector.Z = a.Z * f;
            return vector;
        }

        public static Vector operator / (Vector a, float f)
        {
            Vector vector = new Vector();
            vector.X = a.X / f;
            vector.Y = a.Y / f;
            vector.Z = a.Z / f;
            return vector;
        }

        public static implicit operator Vector(Vector3 _v)
        {
            return new Vector(_v.x, _v.y, _v.z);
        }

        public static implicit operator Vector3(Vector _v)
        {
            return new Vector3(_v.X, _v.Y, _v.Z);
        }
    }
}
