using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FireFly.Utilities
{
    public static class PointExtensions
    {
        public static float DistanceTo(this Point point, Point other)
        {
            float distance2 = point.DistanceTo2(other);

            return Mathf.Sqrt(distance2);
        }

        public static float DistanceTo2(this Point point, Point other)
        {
            int dx = other.X - point.X;
            int dy = other.Y - point.Y;

            return Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2);
        }

        public static Vector3 ToVector3(this Point point)
        {
            return new Vector3(point.X, 0f, point.Y);
        }
    }
}
