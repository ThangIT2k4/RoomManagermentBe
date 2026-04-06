namespace Organization.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}

