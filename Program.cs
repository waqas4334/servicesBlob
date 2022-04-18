using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using System.Linq;
using MySql.Data.MySqlClient;

namespace C_sharp_init
{
    class vehicle
    {
        public string color;
        public string model;
        public int maxSpeed;



    }


    class car : vehicle
    {
        public string modelName = "Mustang";




    }

    internal class Program
    {
        static public void Main(string[] args)
        {


            //vehicle bike = new vehicle();
            //bike.color = "red";
            //bike.model = "125";
            //bike.maxSpeed= 100;

            //vehicle car = new vehicle();
            //car.color = "blue";
            //car.model = "BMW";

            //vehicle Ford = new vehicle();  // Create an object of the Car Class (this will call the constructor)
            //Console.WriteLine(Ford.model);





            //Console.WriteLine("bike color is : " + bike.color  + "\t" + "car color is" + car.color);

            //car c = new car();
            //Console.WriteLine(c.modelName);

            //string writeText = "Hello World!";  // Create a text string
            //File.WriteAllText("filename.txt", writeText);  // Create a file and write the content of writeText to it

            //string readText = File.ReadAllText("filename.txt");  // Read the contents of the file
            //Console.WriteLine(readText);  // Output the content


            //double myDouble = 9.78;
            // int myInt = Convert.ToInt32(myDouble);// Manual casting: double to int

            //       Console.WriteLine(myDouble);
            // Console.WriteLine(myInt);



            //       // Type your username and press enter
            //       Console.WriteLine("Enter username:");

            //       // Create a string variable and get user input from the keyboard and store it in the variable
            //       string userName = Console.ReadLine();

            //       // Print the value of the variable (userName), which will display the input value
            //       Console.WriteLine("Username is: " + userName);

            //Console.WriteLine("Enter your age:");
            //int age = Convert.ToInt32(Console.ReadLine());
            //Console.WriteLine("Your age is: " + age);



            // sort array of numbers using for loop 
            //int[] numbers = { 1, 4, 5, 4, 9, 6, 7, 8, 9, 10 };
            ////for (int i = 0; i < numbers.Length; i++)
            ////{
            ////    for (int j = i + 1; j < numbers.Length; j++)
            ////    {
            ////        if (numbers[i] > numbers[j])
            ////        {
            ////            int temp = numbers[i];
            ////            numbers[i] = numbers[j];
            ////            numbers[j] = temp;

            ////        }
            ////    }
            ////}

            //foreach (int number in numbers)
            //{
            //    Console.WriteLine(number);
            //}


            //for (int i = 0; i < 10; i++)
            //{
            //    if (i == 4 || i == 5)
            //    {
            //        continue;
            //    }


            //    Console.WriteLine(i);
            //}





            Stack<string> stack = new Stack<string>();
            stack.Push("Hello");
            stack.Push("World");
            stack.Push("!");

            foreach (string s in stack)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("Popped: " + stack.Pop());
            Console.WriteLine("Popped: " + stack.Pop());
            Console.WriteLine("Popped: " + stack.Pop());

            foreach (string s in stack)
            {
                Console.WriteLine(s);
            }






        }

















    }
}

















