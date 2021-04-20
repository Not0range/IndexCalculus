using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace IndexCalculus
{
    class GCD
    {
        public BigInteger a;

        public BigInteger b;

        public BigInteger gcd;

        public BigInteger x;

        public BigInteger y;

        public GCD(BigInteger a, BigInteger b)
        {
            if (a > b)
            {
                this.a = a;
                this.b = b;
            }
            else
            {
                this.a = b;
                this.b = a;
            }
            Calculate();
        }

        private void Calculate()
        {
            BigInteger[] u = new BigInteger[] { a, 1, 0 };
            BigInteger[] v = new BigInteger[] { b, 0, 1 };
            BigInteger[] t = new BigInteger[3];
            BigInteger q;

            while (v[0] != 0)
            {
                q = u[0] / v[0];
                t[0] = u[0] % v[0];
                t[1] = u[1] - q * v[1];
                t[2] = u[2] - q * v[2];
                Array.Copy(v, u, 3);
                Array.Copy(t, v, 3);
            }
            gcd = u[0];
            x = u[1];
            y = u[2];
        }

        public static GCD Calculate(BigInteger a, BigInteger b)
        {
            return new GCD(a, b);
        }
    }
}
