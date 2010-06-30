//----------------------------------------
// Copyright (c) 2010, Dmitry Zamkov
// Open source under the new BSD License
//----------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;

namespace DHTW
{
    /// <summary>
    /// The most awesome mandelbox.
    /// </summary>
    public class MandelBox : ISharpShape
    {
        public MandelBox(uint IterMax, double S, double R, double F)
        {
            this._IterMax = IterMax;
            this._S = S;
            this._R = R;
            this._RSQR = this._R * this._R;
            this._F = F;
        }

        public MandelBox(uint IterMax)
            : this(IterMax, 2.0, 0.5, 1.0)
        {

        }

        public bool Occupies(Vector Position)
        {
            uint iternum = 0;
            Vector curpos = Position;
            while (iternum < this._IterMax)
            {
                if (curpos.Length() > Math.Sqrt(3))
                {
                    return false;
                }

                // Box fold
                if (curpos.X > 1.0)
                {
                    curpos.X = 2 - curpos.X;
                }
                else if (curpos.X < -1.0)
                {
                    curpos.X = -2 - curpos.X;
                }
                if (curpos.Y > 1.0)
                {
                    curpos.Y = 2 - curpos.Y;
                }
                else if (curpos.Y < -1.0)
                {
                    curpos.Y = -2 - curpos.Y;
                }
                if (curpos.Z > 1.0)
                {
                    curpos.Z = 2 - curpos.Z;
                }
                else if (curpos.Z < -1.0)
                {
                    curpos.Z = -2 - curpos.Z;
                }
                curpos *= this._F;

                // Ball fold
                double mag = curpos.Length();
                if (mag < this._R)
                {
                    curpos *= 1.0 / this._RSQR;
                }
                else if (mag < 1.0)
                {
                    curpos *= 1.0 / (mag * mag * mag);
                }

                // Constants
                curpos *= this._S;
                curpos = curpos - Position;

                iternum++;
            }
            return true;
        }

        public ISharpShape Subsect(Vector Pos, Vector Neg)
        {
            return new Subsection(this, Pos, Neg);
        }

        private uint _IterMax;
        private double _S;
        private double _R;
        private double _RSQR;
        private double _F;
    }
}