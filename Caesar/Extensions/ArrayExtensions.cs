using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Caesar.Extensions
{
    public static class ArrayExtensions
    {
        extension<TSelf>(TSelf[] array)
            where TSelf : IFloatingPoint<TSelf>
        {
            /// <summary>
            /// <para>Calculates the inner product of the two vectors.</para>
            /// <para>The calculation fails when the vector shapes are mismatched.</para>
            /// </summary>
            /// <param name="selves">The first vector to be multiplied.</param>
            /// <param name="selves1">The second vector to multiply.</param>
            /// <returns>The inner product.</returns>
            /// <exception cref="ArgumentException">The patterns of the vector mismatch.</exception>
            public static TSelf operator *(TSelf[] selves, TSelf[] selves1)
            {
                if (selves.Length != selves1.Length)
                    throw new ArgumentException("Length mismatch.", nameof(selves1));

                TSelf self = TSelf.Zero;

                foreach ((TSelf first, TSelf second) in selves.Zip(selves1))
                {
                    self += first * second;
                }

                return self;
            }
        }
    }
}
