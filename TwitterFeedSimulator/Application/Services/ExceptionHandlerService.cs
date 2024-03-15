using System;
using System.ComponentModel;
using TwitterFeedSimulator.Application.Interfaces;

namespace TwitterFeedSimulator.Application.Services
{
	public class ExceptionHandlerService : IExceptionHandlerService
	{
		public ExceptionHandlerService()
		{
		}

        public void HandleException(Exception exception)
        {
            if (exception is FileNotFoundException)
            {
                Console.WriteLine("File not found. Please check the file path and try again.");
            }
            else if (exception is IOException)
            {
                Console.WriteLine("An I/O error occurred while reading the file. Please try again later.");
            }
            else if (exception is ArgumentException)
            {
                Console.WriteLine("Invalid argument provided. Please check your file input path and try again.");
            }
            else if (exception is UnauthorizedAccessException)
            {
                Console.WriteLine("Access to the file is unauthorized. Please ensure you have the necessary permissions.");
            }
            else
            {
                Console.WriteLine("An unexpected error occurred. Please try again or contact support.");
            }

            throw exception;
        }
    }
}

