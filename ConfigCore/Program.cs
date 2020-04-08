using System;
using System.Configuration;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Key: {ConfigurationManager.AppSettings["My/Key"]}");
    }
}