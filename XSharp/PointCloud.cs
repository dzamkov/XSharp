//----------------------------------------
// Copyright (c) 2010, Dmitry Zamkov
// Open source under the new BSD License
//----------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace DHTW
{
    /// <summary>
    /// A point cloud stored in the graphics card.
    /// </summary>
    public class PointCloud
    {
        public PointCloud()
        {
            this._ChunkSize = 1024;
            this._VerticeSize = sizeof(float) * 3 + 4;
            this._PointGroups = new Dictionary<int, _MemoryRange>();
        }
        
        /// <summary>
        /// Describes a memory location in the gfx card.
        /// </summary>
        private class _MemoryRange
        {
            public uint Buffer;
            public int Index;
            public int Size;
            public _MemoryRange Next;
            public _MemoryRange Prev;

            public float PointSize; // Size of points to draw in this range. 0< = no draw.
        }

        /// <summary>
        /// Represents a point in the point cloud.
        /// </summary>
        public struct Point
        {
            public Vector Position;
            public byte A;
            public byte R;
            public byte G;
            public byte B;
        }

        /// <summary>
        /// Submits a set of points to the point cloud and returns an index to the
        /// resulting point group.
        /// </summary>
        public int Submit(Point[] Points, float PointSize)
        {
            _MemoryRange range = this._Allocate(Points.Length);
            int index = this._NextPointIndex++;
            this._PointGroups[index] = range;
            range.PointSize = PointSize;

            // Write to buffer
            unsafe
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, range.Buffer);
                byte* mem = (byte*)GL.MapBuffer(
                    BufferTarget.ArrayBuffer, 
                    BufferAccess.WriteOnly).ToPointer();
                mem += range.Index * this._VerticeSize;
                for (int t = 0; t < Points.Length; t++)
                {
                    float* posdata = (float*)(mem + 4);
                    Point p = Points[t];
                    mem[0] = p.R;
                    mem[1] = p.G;
                    mem[2] = p.B;
                    mem[3] = p.A;
                    posdata[0] = (float)p.Position.X;
                    posdata[1] = (float)p.Position.Y;
                    posdata[2] = (float)p.Position.Z;
                    
                    mem += this._VerticeSize;
                }
                GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            }

            return index;
        }

        /// <summary>
        /// Removes a point group from rendering.
        /// </summary>
        public void Retract(int Index)
        {
            //TODO: This
        }

        /// <summary>
        /// Renders the point cloud.
        /// </summary>
        public void Render()
        {
            GL.InterleavedArrays(InterleavedArrayFormat.C4ubV3f, 0, IntPtr.Zero);
            foreach (_MemoryRange mr in this._PointGroups.Values)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, mr.Buffer);
                GL.PointSize(mr.PointSize);
                GL.DrawArrays(BeginMode.Points, mr.Index, mr.Size);
            }
        }

        /// <summary>
        /// Allocates an unused memory range of the specified size in points.
        /// </summary>
        private _MemoryRange _Allocate(int Size)
        {
            _MemoryRange cur = this._First;
            while (cur != null)
            {
                if(cur.PointSize < 0.0f)
                {
                    if (cur.Size == Size)
                    {
                        return cur;
                    }
                    if (cur.Size > Size)
                    {
                        // Split memory range, grab first block of the right size
                        _MemoryRange firstpart = new _MemoryRange();
                        _MemoryRange secondpart = new _MemoryRange();
                        firstpart.Index = cur.Index;
                        firstpart.Size = Size;
                        secondpart.Index = firstpart.Index + firstpart.Size;
                        secondpart.Size = cur.Size - Size;
                        firstpart.Buffer = cur.Buffer;
                        secondpart.Buffer = cur.Buffer;
                        firstpart.Next = secondpart;
                        secondpart.Prev = firstpart;
                        if (cur.Next != null)
                        {
                            secondpart.Next = cur.Next;
                            cur.Next.Prev = secondpart;
                        }
                        else
                        {
                            this._Last = secondpart;
                        }
                        if (cur.Prev != null)
                        {
                            firstpart.Prev = cur.Prev;
                            cur.Prev.Next = firstpart;
                        }
                        else
                        {
                            this._First = firstpart;
                        }
                        return firstpart;
                    }
                }
            }

            // OH NOES, not avaiable, lets make a new buffer.
            int targetsize = (this._ChunkSize < Size) ? Size : this._ChunkSize;

            uint buf;
            GL.GenBuffers(1, out buf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(targetsize * this._VerticeSize), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            if (targetsize == Size)
            {
                _MemoryRange mr = new _MemoryRange();
                mr.Prev = this._Last;
                if (this._Last != null)
                {
                    this._Last.Next = mr;
                }
                else
                {
                    this._First = mr;
                }
                this._Last = mr;
                mr.Buffer = buf;
                mr.Index = 0;
                mr.Size = targetsize;
                return mr;
            }
            else
            {
                _MemoryRange firstpart = new _MemoryRange();
                _MemoryRange secondpart = new _MemoryRange();
                firstpart.Prev = this._Last;
                secondpart.Prev = firstpart;
                if (this._Last != null)
                {
                    this._Last.Next = firstpart;
                }
                else
                {
                    this._First = firstpart;
                }
                firstpart.Next = secondpart;
                this._Last = secondpart;
                firstpart.Buffer = secondpart.Buffer = buf;
                firstpart.Index = 0;
                firstpart.Size = Size;
                secondpart.Index = Size;
                secondpart.Size = targetsize - Size;
                return firstpart;
            }
        }

        private int _ChunkSize;
        private int _VerticeSize;
        private _MemoryRange _First;
        private _MemoryRange _Last;
        private int _NextPointIndex;
        private Dictionary<int, _MemoryRange> _PointGroups;
    }
}