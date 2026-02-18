namespace Identity.Application.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}