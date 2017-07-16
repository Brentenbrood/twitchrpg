using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FireFly.Utilities;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;


namespace Generators
{
    public enum Direction
    {
        FORWARD,
        LEFT,
        RIGHT,
        BACK
    }

    public static class DirectionExtensions
    {
        public static Vector2 GetDirectionVector(this Direction direction)
        {
            switch (direction)
            {
                case Direction.FORWARD:
                    return Vector2.up;
                case Direction.LEFT:
                    return Vector2.left;
                case Direction.RIGHT:
                    return Vector2.right;
                case Direction.BACK:
                    return Vector2.down;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public static Direction GetDirection(this Vector2 vector2)
        {
            if(vector2 == Vector2.up)
                return Direction.FORWARD;
            if(vector2 == Vector2.left)
                return Direction.LEFT;
            if(vector2 == Vector2.right)
                return Direction.RIGHT;
            if(vector2 == Vector2.down)
                return Direction.BACK;

            throw new NotImplementedException("Rounding is not implemented yet, please use constant 90 degrees rotations");
        }

        public static Direction RotateDirection(this Direction direction, Direction turn)
        {
            Vector2 current = direction.GetDirectionVector();
            float angle = Vector2.Angle(Vector2.up, turn.GetDirectionVector());

            Vector2 newCurrent = Quaternion.Euler(0, 0, angle) * current;

            return newCurrent.GetDirection();
        }
    }

    /// <summary>
    /// WARNING: HOLY SHIT THIS IS NOT WRITTEN CLEANLY
    /// </summary>
    public class CorridorGenerator : MonoBehaviour
    {
        public float MinCorridor = 2f;
        public float MaxCorridor = 10f;
        public int MinCorridors = 8;
        public int MaxCorridors = 14;
        public float[] ConnectionRanges;

        public int seed = 1233211;
        public bool randomizeSeed = true;

        private Map2 map;
        private Point[] points;

        void Start()
        {
            int corners = Random.Range(MinCorridors, MaxCorridors);
            map = new Map2(75, 75);
            points = new Point[corners];

            if(!randomizeSeed)
                Random.InitState(seed);

            GeneratePoints();

            ConnectPoints();

            //Debug.Log(CreateCorridor(points[0], points[1]));

            CreateWorldGeometry();
        }

        void GeneratePoints()
        {
            for (int i = 0; i < points.Length;)
            {
                int x = Random.Range(0, map.Width);
                int y = Random.Range(0, map.Height);

                if (!PointExists(x, y))
                {
                    map.Set(x, y, 1);
                    points[i] = new Point(x, y);
                    i++;
                }
            }
        }

        bool PointExists(int x, int y)
        {
            foreach (Point point in points)
            {
                if (point.X == x && point.X == y)
                    return true;
            }

            return false;
        }

        private void ConnectPoints()
        {
            List<Point>[] connectedPoints = new List<Point>[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                connectedPoints[i] = new List<Point>();
            }

            for (int i = 0; i < points.Length; i++)
            {
                Point point = points[i];

                List<Point> pointsWithin = GetSortedPointsWithin(point.X, point.Y, ConnectionRanges);
                pointsWithin.Sort(new SortByDistance(point));
                if (pointsWithin.Count > 0)
                    connectedPoints[i] = pointsWithin.GetRange(0, Math.Min(4, pointsWithin.Count));
            }

            //TODO: Should this be in loop before?
            for (int i = 0; i < points.Length; i++)
            {
                foreach (Point point in connectedPoints[i])
                {
                    CreateCorridor(points[i], point);
                }
            }

            //Debug prints
            for (int i = 0; i < points.Length; i++)
            {
                string connections = "";
                foreach (Point point in connectedPoints[i])
                {
                    connections += point + " , ";
                    Debug.DrawLine(point.ToVector3(), points[i].ToVector3(), Color.red, 100f);
                }

                Debug.Log(points[i] + ": " + connections);
            }

        }

        List<Point> GetSortedPointsWithin(int x, int y, float[] radiusPerPoint)
        {
            Point fromPoint = new Point(x, y);
            Vector2 fromVector2 = new Vector2(x, y);

            Point[] sortingPoints = new Point[points.Length];
            Array.Copy(points, sortingPoints, points.Length);
            Array.Sort(sortingPoints, new SortByDistance(fromPoint));

            List<Point> pointsWithin = new List<Point>();
            for (int i = 0; i < sortingPoints.Length && pointsWithin.Count < radiusPerPoint.Length; i++)
            {
                Point toPoint= sortingPoints[i];

                if (Vector2.Distance(fromVector2, new Vector2(toPoint.X, toPoint.Y)) <= radiusPerPoint[pointsWithin.Count] && toPoint != fromPoint )
                {
                    pointsWithin.Add(toPoint);
                }
            }

            return pointsWithin;
        }

        bool CreateCorridor(Point a, Point b)
        {
            bool horizontalFirst = Random.Range(0, 2) == 0;

            if (horizontalFirst)
            {
                if (!CreateHorizontalCorridor(a, b))
                    return CreateVerticalCorridor(a, b);
            }
            else
            {
                if (!CreateVerticalCorridor(a, b))
                    return CreateHorizontalCorridor(a, b);
            }

            return true;
        }

        //TODO: Better name for this
        bool CreateHorizontalCorridor(Point a, Point b)
        {
            bool isValid = CheckHorizontalCorridor(a, b);
            if (!isValid)
                return false;

            //fill horizontal path
            for (int x = Mathf.Min(a.X, b.X); x < Mathf.Max(a.X, b.X); x++)
                map.Set(x, a.Y, 1);

            //fill vertical path
            for (int y = Mathf.Min(a.Y, b.Y); y < Mathf.Max(a.Y, b.Y); y++)
                map.Set(b.X, y, 1);

            return true;
        }

        //TODO: Better name for this
        bool CheckHorizontalCorridor(Point a, Point b)
        {
            //TODO: Remove, this is for debugging:
            return true;

            //check horizontal path
#pragma warning disable CS0162 // Unreachable code detected
            for (int x = Mathf.Min(a.X, b.X); x < Mathf.Max(a.X, b.X); x++)
                if (map.Get(x, a.Y) != 0 && (a.X != x || b.X != x))
                    return false;

            //check vertical path
            for (int y = Mathf.Min(a.Y, b.Y); y < Mathf.Max(a.Y, b.Y); y++)
                if (map.Get(b.X, y) != 0 && (a.Y != y || b.Y != y))
                    return false;

            return true;
#pragma warning restore CS0162 // Unreachable code detected
        }

        bool CreateVerticalCorridor(Point a, Point b)
        {
            bool isValid = CheckVerticalCorridor(a, b);
            if (!isValid)
                return false;

            //fill vertical path
            for (int y = Mathf.Min(a.Y, b.Y); y < Mathf.Max(a.Y, b.Y); y++)
                map.Set(a.X, y, 1);

            //fill horizontal path
            for (int x = Mathf.Min(a.X, b.X); x < Mathf.Max(a.X, b.X); x++)
                map.Set(x, b.Y, 1);

            return true;

        }

        //TODO: Better name for this
        bool CheckVerticalCorridor(Point a, Point b)
        { //TODO: Test this method
            //TODO: Remove, this is for debugging:
            return true;

            //check vertical path
#pragma warning disable CS0162 // Unreachable code detected
            for (int y = Mathf.Min(a.Y, b.Y); y < Mathf.Max(a.Y, b.Y); y++)
                if (map.Get(a.X, y) != 0 && (a.Y != y || b.Y != y))
                    return false;

            //check horizontal path
            for (int x = Mathf.Min(a.X, b.X); x < Mathf.Max(a.X, b.X); x++)
                if (map.Get(x, b.Y) != 0 && (a.X != x || b.X != x))
                    return false;

            return true;
#pragma warning restore CS0162 // Unreachable code detected
        }

        void Update()
        {
            /*Point start = points[0];
            foreach (Point point in points)
            {
                //Debug.DrawLine(new Vector3(start.X, 0f, start.Y), new Vector3(point.X, 0f, point.Y), UnityEngine.Color.red, 0f, false);
                start = point;
            }*/
        }

        void CreateWorldGeometry()
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.Get(x, y) != 0)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.SetParent(transform, false);
                        cube.transform.localPosition = new Vector3(x, 0, y); 
                    }
                }
            }

            Transform platform = transform.Find("Solid Ground");
            platform.transform.localScale = new Vector3(map.Width, 1f, map.Height);
        }

        private class SortByDistance : IComparer<Point>
        {
            public Point FromPoint;

            public SortByDistance(Point fromPoint)
            {
                FromPoint = fromPoint;
            }

            public int Compare(Point p1, Point p2)
            {
                float dist1 = FromPoint.DistanceTo2(p1);
                float dist2 = FromPoint.DistanceTo2(p2);

                if (Math.Abs(dist1 - dist2) < 0.01f)
                    return 0;

                return dist1 > dist2 ? 1 : -1 ;
            }
        }

        #region Unused Code
        //TODO: Remove this class
        //private class PointData
        //{
        //    public Point Point;
        //    public PointData[] Connection;

        //    public PointData()
        //    {
        //    }
        //}

        //void OldGeneratePoints()
        //{
        //    Direction dir = RandomDirection();
        //    Point start = new Point(13, 13);
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        if (i != 0)
        //            start = points[i - 1];

        //        dir = RandomDirection(dir);
        //        Vector2 end = dir.GetDirectionVector() * Random.Range(MinCorridor, MaxCorridor) + start;

        //        points[i] = end;
        //    }
        //}

        Direction RandomDirection()
        {
            int num = Random.Range(0, 4);
            switch (num)
            {
                case 0:
                    return Direction.FORWARD;
                case 1:
                    return Direction.LEFT;
                case 2:
                    return Direction.RIGHT;
                case 3:
                    return Direction.BACK;
                default:
                    throw new Exception("Random.range returns out of range integer");
            }
        }

        Direction RandomDirection(Direction from)
        {
            int num = Random.Range(0, 2);
            Direction relativeDirection = Direction.FORWARD;
            switch (num)
            {
                case 0:
                    relativeDirection = Direction.LEFT;
                    break;
                case 1:
                    relativeDirection = Direction.RIGHT;
                    break;
                default:
                    throw new Exception();
            }

            return @from.RotateDirection(relativeDirection);
        } 
        #endregion

    } 
}
