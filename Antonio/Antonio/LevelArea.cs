using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antonio
{
    class LevelArea
    {
        public List<Box> Boxes;
        public List<Taco> Tacos;
        public LevelArea(List<Box> boxes, List<Taco> tacos)
        {
            Boxes = boxes;
            Tacos = tacos;
        }
    }
}
