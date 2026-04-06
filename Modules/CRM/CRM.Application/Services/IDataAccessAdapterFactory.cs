namespace CRM.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}

