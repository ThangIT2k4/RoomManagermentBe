namespace Finance.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}

