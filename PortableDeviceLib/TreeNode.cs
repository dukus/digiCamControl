#region License
/*
TreeNode.cs
Copyright (C) 2009 Vincent Lainé
 
This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableDeviceLib
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>, ITreeNode
    {

        private List<TreeNode<T>> childs;

        public TreeNode(TreeNode<T> parent)
        {
            this.childs = new List<TreeNode<T>>();
            this.Parent = parent;
        }

        public TreeNode(TreeNode<T> parent, T value)
            : this(parent)
        {
            this.Value = value;
        }

        public TreeNode<T> Parent
        {
            get;
            private set;
        }

        public TreeNode<T> this[int index]
        {
            get
            {
                return this.childs[index];
            }
        }

        public IEnumerable<TreeNode<T>> Childs
        {
            get
            {
                return this.childs;
            }
        }

        public int Count
        {
            get
            {
                return this.childs.Count;
            }
        }

        public T Value
        {
            get;
            set;
        }

        public TreeNode<T> AddChild(T value)
        {
            TreeNode<T> child = new TreeNode<T>(this, value);
            this.AddChild(child);
            return child;
        }

        public void AddChild(TreeNode<T> child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            this.childs.Add(child);
        }

        public void RemoveChild(TreeNode<T> child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            this.childs.Remove(child);
        }

        public TreeNode<T> RemoveChild(int index)
        {
            if (index < 0 || index >= this.childs.Count)
                throw new ArgumentOutOfRangeException("index");

            TreeNode<T> child = this.childs[index];
            this.childs.RemoveAt(index);
            return child;
        }

        #region IEnumerable<TreeNode<T>> Members

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            return this.childs.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.childs.GetEnumerator();
        }

        #endregion
    }
}
