using System;
using System.Drawing;

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
    public double StartX { get; set; } // Nouvelle propriété StartX
    public double StartY { get; set; } // Nouvelle propriété StartY
    public double EndX { get; set; } // Nouvelle propriété EndX
    public double EndY { get; set; } // Nouvelle propriété EndY
    public double RotationAngle { get; set; }
    public int ToursEffectues { get; set; } = 0;
    public int DistanceParcourue { get; set; }


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
        StartX = x; // Initialisation de StartX avec la valeur de x
        StartY = y; // Initialisation de StartY avec la valeur de y
        DistanceParcourue = 0;
        ToursEffectues =0;
    }
  
}
