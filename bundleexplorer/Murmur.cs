/*
The MIT License (MIT)

Copyright (c) 2014 Data.HashFunction Developers

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bundleexplorer
{
    internal class Murmur
    {
        public static ulong ComputeHash64(byte[] data)
        {
            const ulong _mixConstant64 = 0xc6a4a7935bd1e995;
            var dataArray = data;
            var dataOffset = 0;
            var dataCount = data.Length;

            var endOffset = dataOffset + dataCount;
            var remainderCount = dataCount % 8;

            UInt64 hashValue = 0 ^ ((UInt64)dataCount * _mixConstant64);

            // Process 8-byte groups
            {
                var groupEndOffset = endOffset - remainderCount;

                for (var currentOffset = dataOffset; currentOffset < groupEndOffset; currentOffset += 8)
                {
                    UInt64 k = BitConverter.ToUInt64(dataArray, currentOffset);

                    k *= _mixConstant64;
                    k ^= k >> 47;
                    k *= _mixConstant64;

                    hashValue ^= k;
                    hashValue *= _mixConstant64;
                }
            }

            // Process remainder
            if (remainderCount > 0)
            {
                var remainderOffset = endOffset - remainderCount;

                switch (remainderCount)
                {
                    case 7: hashValue ^= (UInt64)dataArray[remainderOffset + 6] << 48; goto case 6;
                    case 6: hashValue ^= (UInt64)dataArray[remainderOffset + 5] << 40; goto case 5;
                    case 5: hashValue ^= (UInt64)dataArray[remainderOffset + 4] << 32; goto case 4;
                    case 4:
                        hashValue ^= (UInt64)BitConverter.ToUInt32(dataArray, remainderOffset);
                        break;

                    case 3: hashValue ^= (UInt64)dataArray[remainderOffset + 2] << 16; goto case 2;
                    case 2: hashValue ^= (UInt64)dataArray[remainderOffset + 1] << 8; goto case 1;
                    case 1:
                        hashValue ^= (UInt64)dataArray[remainderOffset];
                        break;
                };

                hashValue *= _mixConstant64;
            }


            hashValue ^= hashValue >> 47;
            hashValue *= _mixConstant64;
            hashValue ^= hashValue >> 47;

            return hashValue;
        }
    }
}
