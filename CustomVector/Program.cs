using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Tests:");
            TestMagnitude();
            TestAddition();
            TestDot();
            TestCross();
            TestScalarProduct();
            TestAngle();
            Console.WriteLine("Tests Finished");
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Console.WriteLine("Test Failed: " + message);
            }
        }


        public static void TestMagnitude()
        {
            Vector v1 = new Vector(1, 0, 0);
            Vector v2 = new Vector(3, 4, 0);

            Assert(v1.magnitude == 1, "Magnitude Failed");
            Assert(v2.magnitude == 5, "Magnitude Failed");
        }

        public static void TestAddition()
        {
            Vector v1 = new Vector(1, 0, 0);
            Vector v2 = new Vector(1, 1, 0);

            Assert(v1 + v2 == new Vector(2, 1, 0), "Addition Failed");
        }

        public static void TestDot()
        {
            Vector v1 = new Vector(1, 0, 0);
            Vector v2 = new Vector(1, 1, 0);
            Vector v3 = new Vector(5, 5, 5);

            Assert(Vector.Dot(v1, v2) == 1, "Dot Product Failed");
            Assert(Vector.Dot(v2, v3) == 10, "Dot Product Failed");
        }

        public static void TestCross()
        {
            Vector v1 = new Vector(0, 0, 1);
            Vector v2 = new Vector(0, 1, 0);

            Vector v3 = Vector.Cross(v1, v2);

            Assert(v3 == new Vector(1, 0, 0), "CrossProduct Failed");
        }

        public static void TestScalarProduct()
        {
            Vector v1 = new Vector(1, 2, 3);
            float m = 10;
            Assert(v1 * m == new Vector(10, 20, 30), "ScalarMultiplication failed");
            Assert(v1 * m == m * v1, "ScalarMultiplication failed");
        }

        public static void TestAngle()
        {
            Vector v1 = new Vector(0, 0, 1);
            Vector v2 = new Vector(0, 1, 0);

            Assert(Vector.AngleBetween(v1, v2) == 90, "AnbleBetween Failed");
        }

        public static void TestNormalized()
        {
            Vector v1 = new Vector(10, 0, 0);

            Assert(v1.normalized == new Vector(1, 0, 0), "Normalization Failed");
        }
    }
}
