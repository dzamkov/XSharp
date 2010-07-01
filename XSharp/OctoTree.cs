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
    /// The main unit of storage for fractals. Each octotree unit is a cube with 8 child cubes for
    /// each corner and specify where the fractal is in their volumes.
    /// </summary>
    public class OctoTree
    {
        private OctoTree()
        {
            // Self referential for completely full or completely empty
            this.PPP = this;
            this.PPN = this;
            this.PNP = this;
            this.PNN = this;
            this.NPP = this;
            this.NPN = this;
            this.NNP = this;
            this.NNN = this;
        }

        private OctoTree(OctoTree[] Children)
        {
            this.PPP = Children[0];
            this.PPN = Children[1];
            this.PNP = Children[2];
            this.PNN = Children[3];
            this.NPP = Children[4];
            this.NPN = Children[5];
            this.NNP = Children[6];
            this.NNN = Children[7];
        }

        // Gets a sub octotree at the specified position.
        public OctoTree this[int Key]
        {
            get
            {
                if (Key < 4)
                {
                    if (Key < 2)
                    {
                        if (Key < 1)
                        {
                            return this.PPP;
                        }
                        else
                        {
                            return this.PPN;
                        }
                    }
                    else
                    {
                        if (Key < 3)
                        {
                            return this.PNP;
                        }
                        else
                        {
                            return this.PNN;
                        }
                    }
                }
                else
                {
                    if (Key < 6)
                    {
                        if (Key < 5)
                        {
                            return this.NPP;
                        }
                        else
                        {
                            return this.NPN;
                        }
                    }
                    else
                    {
                        if (Key < 7)
                        {
                            return this.NNP;
                        }
                        else
                        {
                            return this.NNN;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets an array of children octotree's for this octotree.
        /// </summary>
        public OctoTree[] Children
        {
            get
            {
                return new OctoTree[] {
                    this.PPP,
                    this.PPN,
                    this.PNP,
                    this.PNN,
                    this.NPP,
                    this.NPN,
                    this.NNP,
                    this.NNN
                };
            }
        }

        /// <summary>
        /// Gets an octotree with the specified children.
        /// </summary>
        public static OctoTree Get(OctoTree[] Children)
        {
            uint hash = _CalcHash(Children);

            object hasheditem = _Hashes[hash];
            if (hasheditem == null)
            {
                OctoTree newtree = new OctoTree(Children);
                double filllevel = 0.0;
                for (int t = 0; t < 8; t++)
                {
                    filllevel += (Children[t]._FillLevel) / 8.0;
                }
                newtree._FillLevel = filllevel;
                newtree._Hash = hash;
                _Hashes[hash] = newtree;
                return newtree;
            }
            else
            {
                return _Hashes[hash] as OctoTree;
            }
        }

        /// <summary>
        /// Gets the offset of a child at the specified index.
        /// </summary>
        public static Vector Offset(int Index)
        {
            Vector offset;
            if (Index % 2 < 1)
            {
                offset.Z = 0.5;
            }
            else
            {
                offset.Z = -0.5;
            }
            if (Index % 4 < 2)
            {
                offset.Y = 0.5;
            }
            else
            {
                offset.Y = -0.5;
            }
            if (Index % 8 < 4)
            {
                offset.X = 0.5;
            }
            else
            {
                offset.X = -0.5;
            }
            return offset;
        }

        /// <summary>
        /// Creates an octotree representation of a shape with the specified resolution(Tree depth).
        /// </summary>
        public static OctoTree Create(ISharpShape Shape, int Resolution)
        {
            if (Resolution > 0)
            {
                OctoTree[] children = new OctoTree[8];
                for (int t = 0; t < 8; t++)
                {
                    children[t] = Create(Shape.Subsect(Offset(t), new Vector(0.5, 0.5, 0.5)), Resolution - 1);
                }
                return Get(children);
            }
            else
            {
                if (Shape.Occupies(new Vector(0.0, 0.0, 0.0)))
                {
                    return _Full;
                }
                else
                {
                    return _Empty;
                }
            }
        }
        
        /// <summary>
        /// Computes the hash for an octotree with the specified children.
        /// </summary>
        private static uint _CalcHash(OctoTree[] Children)
        {
            uint hash = 0x1234ABCD;
            for (uint t = 0; t < 8; t++)
            {
                hash += Children[t]._Hash;
                hash += t * 2 + 1;
                hash = hash << 1;
            }
            return hash;
        }

        static OctoTree()
        {
            _Full = new OctoTree();
            _Full._Hash = 0xDEADBEEF;
            _Full._FillLevel = 1.0;
            _Empty = new OctoTree();
            _Empty._Hash = 0x1337BED5;
            _Empty._FillLevel = 0.0;
            _Hashes = new Hashtable();
            _Hashes.Add(_Full._Hash, _Full);
            _Hashes.Add(_Empty._Hash, _Empty);
            _Hashes.Add(_CalcHash(_Full.Children), _Full);
            _Hashes.Add(_CalcHash(_Empty.Children), _Empty);
        }

        /// <summary>
        /// Gets a completely occupied octotree.
        /// </summary>
        public static OctoTree Full
        {
            get
            {
                return _Full;
            }
        }

        /// <summary>
        /// Gets a completely unoccupied octotree.
        /// </summary>
        public static OctoTree Empty
        {
            get
            {
                return _Empty;
            }
        }

        private static OctoTree _Full;
        private static OctoTree _Empty;
        private static Hashtable _Hashes;

        private uint _Hash;
        private double _FillLevel;

        // Arrays are too big and slow for this job
        public OctoTree PPP;
        public OctoTree PPN;
        public OctoTree PNP;
        public OctoTree PNN;
        public OctoTree NPP;
        public OctoTree NPN;
        public OctoTree NNP;
        public OctoTree NNN;
    }
}