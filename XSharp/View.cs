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
    /// A view of a zone.
    /// </summary>
    public class View
    {
        public View()
        {

        }

        /// <summary>
        /// Repositions the camera in the specified zone with the specified
        /// camera-space to zone-space transform.
        /// </summary>
        public void Reposition(Zone Zone, Matrix Transform)
        {
            this._Zone = Zone;
            this._CameraTransform = Transform;
        }

        /// <summary>
        /// Resizes the target display size.
        /// </summary>
        public void Resize(int Width, int Height)
        {
            this._Width = Width;
            this._Height = Height;
        }

        private int _Width;
        private int _Height;
        private Zone _Zone;
        private Matrix _CameraTransform;
    }
}
