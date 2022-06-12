using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using polyfactor.Extensions;

namespace polyfactor.GaloisStructs
{
    public class GF
    {
        public ulong Order { get; private set; }
        private readonly ulong[] _inverses;

        private readonly object _threadLock = new object();

        public GF(ulong order, bool forceCheck = true)
        {
            if (order < 2)
                throw new Exception($"The GF({order}) isn't a Galois field, the min order is 2!");

            Order = order;
            _inverses = new ulong[order];

            // cause in any GF(p) field, the inverse for identity is the identity itself
            _inverses[1] = 1UL;

            if (forceCheck)
            {
                var residues = GetResidue(2, Order);

                ulong stopIterationIndex = 0;

                // take each residues and check if there is an inverse for it
                // do it in parallel
                var plr = Parallel.ForEach(residues, (i, state) =>
                {
                    var (gcd, _, y) = order.ExtendedGCD(i);

                    if (gcd != 1UL)
                    {
                        state.Stop();

                        lock (_threadLock)
                        {
                            stopIterationIndex = i;
                        }

                        return;
                    }

                    if (state.IsStopped)
                        return;

                    _inverses[i] = y >= 0L ? (ulong)y : (ulong)y + order;
                });

                if (!plr.IsCompleted)
                    throw new Exception($"The GF({order}) isn't a Galois field, because there is no inverse for {stopIterationIndex}");
            }
        }

        private static IEnumerable<ulong> GetResidue(ulong min, ulong max)
        {
            for (ulong i = min; i < max; ++i)
                yield return i;
        }

        // works only for fields (it's not working for rings)
        public ulong Inverse(ulong a)
        {
            if (a == 0UL)
                throw new Exception("There is no inverse element for 0!");

            var c = a < Order ? a : a % Order;

            lock (_threadLock)
            {
                var inv = _inverses[c];

                if (inv == 0UL)
                {
                    var (gcd, _, y) = Order.ExtendedGCD(c);

                    if (gcd != 1UL)
                        throw new Exception($"The GF({Order}) isn't a Galois field, because there is no inverse for {c}");

                    _inverses[c] = inv = y >= 0L ? (ulong)y : (ulong)y + Order;
                }

                return inv;
            }
        }

        public string ToLatex() => $@"GF\left({Order}\right)";

        public static bool operator ==(GF gf1, GF gf2) => gf1.Order == gf2.Order;
        public static bool operator !=(GF gf1, GF gf2) => gf1.Order != gf2.Order;

        public override bool Equals(object obj) => obj is GF gf && (this == gf);
        public override int GetHashCode() => Order.GetHashCode();
        public override string ToString() => Order.ToString();
    }
}
