//----------------------------------------
// Copyright (c) 2010, Dmitry Zamkov 
// Open source under the new BSD License
//----------------------------------------
using System;
using System.Collections.Generic;
using OpenTK;

namespace DHTW
{
    /// <summary>
    /// An area divided like an octotree, with additional entity, visual and spatial information.
    /// </summary>
    public class Zone
    {
        private Zone()
        {

        }

        public Zone(IBlockyShape Volume)
        {
            this._Volume = Volume;
        }

        /// <summary>
        /// Creates the child zones for this zone.
        /// </summary>
        public void Split()
        {
            if (this._Children == null)
            {
                this._Children = new Zone[8];
                IBlockyShape[] volchildren = this._Volume.Children;
                for (int t = 0; t < 8; t++)
                {
                    Zone child = this._Children[t] = new Zone();
                    child._Parent = this;
                    child._Volume = volchildren[t];
                    child._Borders = new Zone[6];
                }
                for (int t = 0; t < 4; t++)
                {
                    this._Children[t * 2 + 0]._SetBorder(0, this._Children[t * 2 + 1]);
                    this._Children[t * 2 + 1]._SetBorder(1, this._Children[t * 2 + 0]);
                    this._Children[t + (t / 2) * 2 + 0]._SetBorder(2, this._Children[t + (t / 2) * 2 + 2]);
                    this._Children[t + (t / 2) * 2 + 2]._SetBorder(3, this._Children[t + (t / 2) * 2 + 0]);
                    this._Children[t + 0]._SetBorder(4, this._Children[t + 4]);
                    this._Children[t + 4]._SetBorder(5, this._Children[t + 0]);
                }
            }
        }

        private void _SetBorder(int Index, Zone Border)
        {
            this._Borders[Index] = Border;
        }

        private IBlockyShape _Volume; // Volume info. If children specify contridicating info, it takes priority.
        private Zone _Parent;
        private Zone[] _Children;
        private Zone[] _Borders;
    }
}
