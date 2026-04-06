namespace Property.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}

