namespace Lease.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}

