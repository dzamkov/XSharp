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
    /// A possibly infinitely complex shape where each point can be determined to be occupied or
    /// unoccupied with some degree of precision.
    /// </summary>
    public interface ISharpShape
    {
        /// <summary>
        /// Gets if the shape occupies the specified point.
        /// </summary>
        bool Occupies(Vector Position);

        /// <summary>
        /// Creates an orthagonal subsection of the shape in the specified rectangular box.
        /// </summary>
        ISharpShape Subsect(Vector Pos, Vector Neg);
    }

    /// <summary>
    /// A sphere of radius 1.
    /// </summary>
    public class Sphere : ISharpShape
    {
        public bool Occupies(Vector Position)
        {
            return Position.Length() < 1.0;
        }

        public ISharpShape Subsect(Vector Pos, Vector Neg)
        {
            return new Subsection(this, Pos, Neg);
        }
    }

    /// <summary>
    /// Subsection of another shape.
    /// </summary>
    public class Subsection : ISharpShape
    {
        public Subsection(ISharpShape Source, Vector Pos, Vector Neg)
        {
            this._Source = Source;
            this._Scale = (Pos - Neg) * 0.5;
            this._Mid = Neg + this._Scale;
        }

        public bool Occupies(Vector Position)
        {
            Position.Scale(this._Scale);
            Position.Add(this._Mid);
            return this._Source.Occupies(Position);
        }

        public ISharpShape Subsect(Vector Pos, Vector Neg)
        {
            Pos.Scale(this._Scale);
            Neg.Scale(this._Scale);
            Pos.Add(this._Mid);
            Neg.Add(this._Mid);
            return this._Source.Subsect(Pos, Neg);
        }

        private ISharpShape _Source;
        private Vector _Scale;
        private Vector _Mid;
    }
}