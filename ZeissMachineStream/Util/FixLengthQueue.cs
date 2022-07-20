using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeissMachineStream.Util
{
    public sealed class FixLengthQueue<T> : Queue<T>
    {
        private int length = 1024;

        public FixLengthQueue(int length) : base(length)
        {
            this.length = length;
        }

        public new void Enqueue(T item)
        {
            if (base.Count == length)
                base.Dequeue();

            base.Enqueue(item);
        }
    }
}
