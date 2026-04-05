namespace Auth.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}

