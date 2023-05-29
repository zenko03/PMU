using System;
using System.Drawing;

namespace PMU.models
{
    public class Horses
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }
        public int Speed { get; set; }
        public int Endurance { get; set; }

        public Horses(string name, int x, int y, int width, int height, Color color, int speed, int endurance)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            Speed = speed;
            Endurance = endurance;
        }
    }
}
