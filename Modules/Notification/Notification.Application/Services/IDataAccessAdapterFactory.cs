namespace Notification.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}
