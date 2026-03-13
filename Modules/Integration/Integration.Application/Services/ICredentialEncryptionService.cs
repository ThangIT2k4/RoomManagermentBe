namespace Integration.Application.Services;

public interface ICredentialEncryptionService
{
    string Encrypt(string plainText);

    string Decrypt(string cipherText);
}
