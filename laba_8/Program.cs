using System;
using System.Numerics;

class SecureCommunication
{
    static Random randomizer = new Random();

    static BigInteger GeneratePrimeNumber(int bitLength)
    {
        while (true)
        {
            BigInteger potentialPrime = GenerateRandomBigInteger(bitLength);

            if (IsProbablyPrime(potentialPrime, 5))
                return potentialPrime;
        }
    }

    static BigInteger GenerateRandomBigInteger(int bitLength)
    {
        byte[] bytes = new byte[bitLength / 8];
        randomizer.NextBytes(bytes);

        return new BigInteger(bytes);
    }

    static bool IsProbablyPrime(BigInteger number, int iterations)
    {
        if (number <= 1 || number == 4)
            return false;
        if (number <= 3)
            return true;

        BigInteger tempNumber = number - 1;
        while (tempNumber % 2 == 0)
            tempNumber /= 2;

        for (int i = 0; i < iterations; i++)
        {
            if (!WitnessTest(GenerateRandomBigInteger(number.ToByteArray().Length - 1), number, tempNumber))
                return false;
        }

        return true;
    }

    static bool WitnessTest(BigInteger a, BigInteger n, BigInteger d)
    {
        BigInteger x = BigInteger.ModPow(a, d, n);
        if (x == 1 || x == n - 1)
            return true;

        while (d != n - 1)
        {
            x = BigInteger.ModPow(x, 2, n);
            d *= 2;

            if (x == 1)
                return false;
            if (x == n - 1)
                return true;
        }

        return false;
    }

    static BigInteger GenerateGenerator(BigInteger prime)
    {
        BigInteger generator;
        do
        {
            generator = GenerateRandomBigInteger(prime.ToByteArray().Length - 1);
        } while (generator < 2 || generator >= prime - 1 || BigInteger.ModPow(generator, 2, prime) == 1 || BigInteger.ModPow(generator, (prime - 1) / 2, prime) == 1);

        return generator;
    }

    static void Main()
    {
        int bitLength = 256;
        BigInteger primeNumber = GeneratePrimeNumber(bitLength);
        Console.WriteLine("Згенероване просте число (p): " + primeNumber);

        BigInteger generator = GenerateGenerator(primeNumber);
        Console.WriteLine("Згенерований генератор (g): " + generator);

        BigInteger privateA = GenerateRandomBigInteger(bitLength);
        BigInteger privateB = GenerateRandomBigInteger(bitLength);

        if (generator < 0 || privateA < 0 || primeNumber <= 0)
        {
            Console.WriteLine("Неправильнi значення для генератора ");
            return;
        }

        BigInteger publicA = BigInteger.ModPow(generator, privateA, primeNumber);
        BigInteger publicB = BigInteger.ModPow(generator, privateB, primeNumber);

        BigInteger sharedSecretA = BigInteger.ModPow(publicB, privateA, primeNumber);
        BigInteger sharedSecretB = BigInteger.ModPow(publicA, privateB, primeNumber);

        Console.WriteLine("Ключ1 - ");
        Console.WriteLine( publicA);
        Console.WriteLine("Ключ2 - ");
        Console.WriteLine(publicB);
        Console.WriteLine("спiльний секретний ключ 1: " + sharedSecretA);
        Console.WriteLine("спiльний секретний ключ 2: " + sharedSecretB);
    }
}