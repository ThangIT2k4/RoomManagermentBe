namespace Auth.Application.Services;

/// <summary>
/// Creates database adapter instances (each with its own connection string).
/// The primary adapter for the request is registered in DI; call <see cref="CreateAdapter"/> when you need extra connections (parallel reads, future multi-adapter joins). Dispose the result when done.
/// </summary>
public interface IDataAccessAdapterFactory
{
    /// <summary>Creates a new adapter. Dispose when finished.</summary>
    IDisposable CreateAdapter();

    /// <summary>Same as <see cref="CreateAdapter"/> — explicit alias for opening a second (or more) connection alongside the primary scoped adapter.</summary>
    IDisposable CreateSecondaryAdapter() => CreateAdapter();
}

