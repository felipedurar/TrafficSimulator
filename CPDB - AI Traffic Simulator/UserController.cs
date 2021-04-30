using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficSimulator
{
    public class UserController
    {
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public UserController()
        {
            Clear();
        }

        public void Clear()
        {
            Up = false;
            Down = false;
            Left = false;
            Right = false;
        }

        public bool IsSomeButtonPressed()
        {
            return (Up == true || Down == true || Left == true || Right == true);
        }
    }
}
