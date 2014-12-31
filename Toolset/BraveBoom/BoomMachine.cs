using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Dreambuild.Games.BraveBoom
{
    public enum BoomStatus
    {
        Ready,
        Booming,
        Stopped,
        BlowedUp
    }

    public class BoomMachine
    {
        public double BoomValue { get; set; }
        public double MaxValue { get; set; }
        public double BoomVel { get; set; }
        public double ShrinkVel { get; set; }
        public double SpringLength { get; set; }
        public BoomStatus Status { get; set; }
        public long Count { get; set; }
        public double BoomAcc
        {
            get
            {
                return SpringLength * 11111;
            }
        }
        public double ShrinkAcc
        {
            get
            {
                return (1 - SpringLength) * 11111;
            }
        }

        public event Action Updated;
        protected void OnUpdated()
        {
            if (Updated != null)
            {
                Updated();
            }
        }

        private Timer _timer;
        public const double FPS = 1000;

        public BoomMachine()
        {
            Reset();
            _timer = new Timer(1 / FPS * 1000);
            _timer.Elapsed += (sender, e) => Update();
        }

        public void Start()
        {
            if (Status == BoomStatus.Ready)
            {
                _timer.Start();
                Status = BoomStatus.Booming;
            }
        }

        public void Stop()
        {
            if (Status == BoomStatus.Booming)
            {
                _timer.Stop();
                Status = BoomStatus.Stopped;
            }
        }

        public void Reset()
        {
            Stop();
            BoomValue = 0;
            MaxValue = 1e6;
            BoomVel = 1e6;
            ShrinkVel = 0;
            SpringLength = 1;
            Status = BoomStatus.Ready;
        }

        private void Update()
        {
            double dt = 1 / FPS;
            BoomVel += BoomAcc * dt;
            ShrinkVel += ShrinkAcc * dt;
            BoomValue += BoomVel * dt;
            MaxValue -= ShrinkVel * dt;
            Count++;
            OnUpdated();

            if (BoomValue > MaxValue)
            {
                _timer.Stop();
                Status = BoomStatus.BlowedUp;
            }
        }
    }
}
