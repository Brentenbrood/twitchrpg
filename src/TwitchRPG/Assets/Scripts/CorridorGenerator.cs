using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using FireFly.Utilities;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public class CorridorGenerator : MonoBehaviour
    {
        public float MinCorridor = 2f;
        public float MaxCorridor = 10f;
        public int NumCorridors = 14;

        private Map2 map;
        private Point[] points;

        void Start()
        {
            int corners = Random.Range(3, 7);
            map = new Map2(25, 25);
            points = new Point[NumCorridors];

            GeneratePoints();
        }

        void GeneratePoints()
        {
            for (int i = 0; i < NumCorridors; )
            {
                int x = Random.Range(0, map.Width);
                int y = Random.Range(0, map.Height);

                if (PointExists(x, y))
                {
                    
                }
                else
                {
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

        void Update()
        {
            Point start = points[0];
            foreach (Point point in points)
            {
                Debug.DrawLine(new Vector2(start.X, start.Y), new Vector2(point.X, point.Y));
                start = point;
            }
        }

        void OldGeneratePoints()
        {
            Direction dir = RandomDirection();
            Point start = new Point(13, 13);
            for (int i = 0; i < points.Length; i++)
            {
                if (i != 0)
                    start = points[i - 1];

                dir = RandomDirection(dir);
                Vector2 end = dir.GetDirectionVector() * Random.Range(MinCorridor, MaxCorridor) + start;

                points[i] = end;
            }
        }
    } 
}
