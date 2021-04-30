using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficSimulator
{
    public class PathGroup
    {
        public int Id { get; set; }
        public List<Path> Paths { get; set; }

        public PathGroup()
        {
            Paths = new List<Path>();
        }

        public Path GetWayWithDir(bool dir)
        {
            for (int C = 0; C < Paths.Count; C++)
            {
                if (Paths[C].Direction == dir)
                {
                    return Paths[C];
                }
            }
            return null;
        }

        public List<Path> GetWaysWithDir(bool dir)
        {
            List<Path> ways = new List<Path>();
            for (int C = 0; C < Paths.Count; C++)
            {
                if (Paths[C].Direction == dir)
                {
                    ways.Add(Paths[C]);
                    //return Paths[C];
                }
            }
            return ways;
        }

    }
}
