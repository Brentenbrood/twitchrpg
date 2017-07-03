using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace FireFly.Utilities
{
    public class Map2
    {
        private byte[,] tiles;

        public int Width { get { return tiles.GetLength(0); } }
        public int Height { get { return tiles.GetLength(1); } }
        public Size Size { get { return new Size(Width, Height); } }
        public Rectangle Rect { get { return new Rectangle(0, 0, Width, Height); } }

        public Map2(int width, int height)
        {
            tiles = new byte[width, height];
        }

        public byte Get(int x, int y)
        {
            if(!InRange(x, y))
                throw new ArgumentOutOfRangeException();

            return tiles[x, y];
        }

        public byte Get(Point point)
        {
            return Get(point.X, point.Y);
        }

        public void Set(int x, int y, byte val)
        {
            if (!InRange(x, y))
                throw new ArgumentOutOfRangeException();

            tiles[x, y] = val;
        }

        public void Set(Point point, byte val)
        {
            Set(point.X, point.Y, val);
        }

        public void SetRect(Rectangle rect, byte val)
        {
            if(!InRange(rect.Location))
                throw new ArgumentOutOfRangeException();

            for (int x = rect.X; x < rect.Width; x++)
            {
                for (int y = rect.Y; y < rect.Height; y++)
                {
                    tiles[x, y] = val;
                }
            }
        }

        /// <summary>
        /// Makes the rectangle valid inside the map
        /// </summary>
        /// <param name="rect">rectangle to clamp</param>
        /// <returns>A valid rectangle inside of the map or a rectangle of (0, 0, -1, -1) if nothing is inside of the map</returns>
        public Rectangle ClampRectangle(Rectangle rect)
        {
            if(!rect.IntersectsWith(Rect))
                return new Rectangle(0, 0, -1, -1);

            rect.Intersect(Rect);
            return rect;
        }

        public void SetLine(int x1, int y1, int x2, int y2, byte val)
        {
            Vector2 start = new Vector2(x1, y1);
            Vector2 end = new Vector2(x2, y2);
            //TODO: Add InRange check
            Vector2 t = new Vector2(x1, y1);
            
            float frac = 1 / Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2));
            float ctr = 0;

            while ((int)t.x != (int)x2 || (int)t.y != (int)y2)
            {
                t = Vector2.Lerp(start, end, ctr);
                ctr += frac;
                tiles[(int)t.x, (int)t.y] = val;
            }
        }

        public void SetHorizontalLine(Point p, int amount, byte val)
        {
            //TODO: Add a InRange check
            int to = p.Y + amount;
            int increase = amount > 0 ? 1 : -1;
            for (int x = 0; x < to; x += increase)
            {
                tiles[p.X, x] = val;
            }
        }

        public void SetVerticalLine(Point p, int amount, byte val)
        {
            //TODO: Add a InRange check
            int to = p.X + amount;
            int increase = amount > 0 ? 1 : -1;
            for (int y = 0; y < to; y += increase)
            {
                tiles[p.X, y] = val;
            }
        }

        public bool InRange(Point point)
        {
            return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
        }

        public bool InRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }

}