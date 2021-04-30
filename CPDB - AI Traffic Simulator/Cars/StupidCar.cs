using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TrafficSimulator.Cars
{
    public class StupidCar : Car
    {
        public StupidCar() : base()
        {
            Color = Color.LightGreen;
            MaximumSpeed = 4.0f;
            SecureDistance = 100.00f;
            SDRatio = 30.0f;
            MSecureDistance = 40.0f;
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
