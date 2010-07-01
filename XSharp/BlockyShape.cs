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
    /// A shape created from blocky octotrees.
    /// </summary>
    public interface IBlockyShape : ISharpShape
    {
        /// <summary>
        /// Gets the 8 children of this shape.
        /// </summary>
        IBlockyShape[] Children { get; }

        /// <summary>
        /// Gets the content of this blocky shape.
        /// </summary>
        BlockyShapeContent Content { get; }
    }

    /// <summary>
    /// Content of a blocky shape.
    /// </summary>
    public enum BlockyShapeContent
    {
        Partial,
        Full,
        Empty
    }
}