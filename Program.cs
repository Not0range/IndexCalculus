using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace IndexCalculus
{
    static class Program
    {
        static void Main(string[] args)
        {
            //int[,] arr = new int[,] { { -1, 2, -2 }, { 2, -1, 5 }, { 3, -2, 4 } };
            //arr = arr.AlgebraicAdd();
            Console.WriteLine("Введите a");
            BigInteger a = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("Введите p");
            BigInteger p = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("Введите y");
            BigInteger y = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("Введите t");
            int t = int.Parse(Console.ReadLine());

            CommonDigits common = new CommonDigits();
            BigInteger[] commonDigits = common.GetNext(t);

            BigInteger[,] system = new BigInteger[t, t];
            BigInteger[] result = new BigInteger[t];
            BigInteger[] divider;
            int r = 0;

            for(int i = 1; r < t; i++)
            {
                divider = BigInteger.ModPow(a, i, p).Divide(commonDigits);
                if (divider == null)
                    continue;
                int j = r - 1;
                for (; j >= 0; j--)
                {
                    bool b = false;
                    BigInteger d = 0;
                    int k = 0;
                    for(; k < t; k++)
                    {
                        if (divider[k] == 0 && system[j, k] == 0)
                            continue;
                        if (divider[k] == 0 || system[j, k] == 0)
                        {
                            b = true;
                            break;
                        }
                        d = divider[0] / system[j, k];
                        break;
                    }
                    if (b)
                        continue;

                    try
                    {
                        for (; k < t; k++)
                        {
                            if (divider[k] == 0 && system[j, k] == 0)
                                continue;
                            if (d != divider[k] / system[j, k])
                                break;
                        }
                    }
                    catch (DivideByZeroException)
                    {
                        continue;
                    }
                    if (k == t)
                        break;
                }
                if (j >= 0)
                    continue;

                for (j = 0; j < t; j++)
                    system[r, j] = divider[j];
                result[r] = i;
                r++;
            }

            for(int i = 0; i < result.Length; i++)
            {
                Console.Write("{0} =", result[i]);
                bool first = false;
                for(int j = 0; j < system.GetLength(1); j++)
                {
                    if (system[i, j] == 0)
                        continue;
                    if(first)
                        Console.Write(" + {0}U{1}", (system[i, j] != 1 ? system[i, j].ToString() : ""), j + 1);
                    else
                    {
                        Console.Write(" {0}U{1}", (system[i, j] != 1 ? system[i, j].ToString() : ""), j + 1);
                        first = true;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            BigInteger[] roots = CalculateRoots(system, result, p);
            for (int i = 0; i < roots.Length; i++)
                Console.WriteLine("U{0} = {1}", i + 1, roots[i]);
            Console.WriteLine();

            BigInteger pow = 1;
            for (; ; pow++)
            {
                divider = (y * BigInteger.ModPow(a, pow, p) % p).Divide(commonDigits);
                if (divider != null)
                    break;
            }

            BigInteger x = -pow;
            for(int i = 0; i < divider.Length; i++)
                x += divider[i] * roots[i];
            x = x.Mod(p - 1);

            Console.WriteLine("x = {0}", x);
            Console.ReadKey();
        }

        public static bool Common(this BigInteger digit)
        {
            for (BigInteger i = 2; i <= digit / 2; i += i.IsEven ? 1 : 2)
                if (digit % i == 0)
                    return false;
            return true;
        }

        public static BigInteger[] Divide(this BigInteger digit, BigInteger[] divider)
        {
            BigInteger[] dividerCount = new BigInteger[divider.Length];
            for(int i = 0; i < divider.Length;)
            {
                if (digit % divider[i] != 0)
                    i++;
                else
                {
                    digit /= divider[i];
                    dividerCount[i]++;
                }
            }
            if(digit == 1)
                return dividerCount;
            return null;
        }

        public static BigInteger Mod(this BigInteger digit, BigInteger p)
        {
            if (digit >= 0)
                return digit % p;
            while (digit < 0)
                digit += p;
            return digit;
        }

        private static BigInteger[] CalculateRoots(BigInteger[,] system, BigInteger[] result, BigInteger p)
        {
            BigInteger[] roots = new BigInteger[result.Length];
            bool[] solved = new bool[result.Length];

            TrySolve(system, result, p, roots, solved);

            if (solved.All(i => i))
                return roots;

            int[] signs = new int[result.Length];
            for (int i = 0; i < signs.Length - 2; i++)
                signs[i] = 2;
            signs[signs.Length - 2] = 1;
            signs[signs.Length - 1] = 1;

            BigInteger[,] systemMod;
            BigInteger[] resultMod;
            do
            {
                systemMod = new BigInteger[1, system.GetLength(1)];
                resultMod = new BigInteger[1];
                for(int i = 0; i < system.GetLength(0); i++)
                {
                    if (signs[i] == 2)
                        continue;

                    for(int j = 0; j < system.GetLength(1); j++)
                    {
                        if (signs[i] == 1)
                            systemMod[0, j] += system[i, j];
                        else
                            systemMod[0, j] -= system[i, j];
                    }
                    if(signs[i] == 1)
                        resultMod[0] += result[i];
                    else
                        resultMod[0] -= result[i];
                }

                if (resultMod[0] < 0)
                {
                    resultMod[0] *= -1;
                    for (int j = 0; j < systemMod.GetLength(1); j++)
                        systemMod[0, j] *= -1;
                }
                TrySolve(systemMod, resultMod, p, roots, solved);
                if (solved.All(i => i))
                    break;
            } 
            while (RecursiveMinus(signs, signs.Length - 1));
            if (solved.All(i => !i))
                throw new ArithmeticException("Корни не найдены");
            bool change;

            do
            {
                change = false;
                for (int l = 0; l < roots.Length; l++)
                {
                    if (!solved[l])
                        continue;

                    for (int i = 0; i < system.GetLength(0); i++)
                    {
                        int k = -1;
                        if (system[i, l] == 0)
                            continue;
                        int j = 0;
                        for (; j < system.GetLength(1); j++)
                        {
                            if (j == l)
                                continue;
                            if (system[i, j] != 0)
                            {
                                if (k == -1)
                                    k = j;
                                else
                                    break;
                            }
                        }
                        if (j == system.GetLength(1) && k != -1 && !solved[k])
                        {
                            roots[k] = (result[i] - roots[l] * system[i, l] / system[i, k]).Mod(p - 1);
                            solved[k] = true;
                            if (solved.All(s => s))
                                break;
                            change = true;
                        }
                    }
                }
                if (solved.All(i => i))
                    break;
            } while (change);

            return roots;
        }

        private static void TrySolve(BigInteger[,] system, BigInteger[] result, BigInteger p, BigInteger[] roots, bool[] solved)
        {
            for (int i = 0; i < system.GetLength(0); i++)
            {
                if (solved[i])
                    continue;
                int j = 0;
                int k = -1;
                for (; j < system.GetLength(1); j++)
                {
                    if (system[i, j] != 0)
                    {
                        if (k == -1)
                            k = j;
                        else
                            break;
                    }
                }
                if (j == system.GetLength(1) && k != -1)
                {
                    roots[k] = result[i] * GCD.Calculate(system[i, k], p - 1).y.Mod(p - 1) % (p - 1);
                    solved[k] = true;
                }
            }
        }

        private static bool RecursiveMinus(int[] array, int index)
        {
            if(array[index] == 0)
            {
                array[index] = 2;
                if (index != 0)
                    return RecursiveMinus(array, index - 1);
                else
                    return false;
            }
            array[index]--;
            return true;
        }
    }

    class CommonDigits
    {
        BigInteger current = 1;

        public BigInteger Current { get => current; }

        public void Reset()
        {
            current = 1;
        }

        public BigInteger GetNext()
        {
            for (current += 1; ; current += current.IsEven ? 1 : 2)
                if (current.Common())
                    return current;
        }

        public BigInteger[] GetNext(int count)
        {
            BigInteger[] digits = new BigInteger[count];
            for (int i = 0; i < count; i++)
                digits[i] = GetNext();
            return digits;
        }
    }
}
