using System;
namespace TwitterFeedSimulator.Application.Interfaces
{
	public interface IExceptionHandlerService
	{
        void HandleException(Exception ex);
    }
}

