using System;
using System.Windows.Controls;

namespace Game2D.Engine
{
    public class BossArena : GameWorld
    {
        public BossArena(Canvas canvas) : base(canvas)
        {
        }

        public (double x, double y) GetStartPosition() => (100, 100);
    }
} 