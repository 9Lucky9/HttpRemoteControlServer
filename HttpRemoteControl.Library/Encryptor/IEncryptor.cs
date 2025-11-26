namespace HttpRemoteControl.Library.Encryptor;

public interface IEncryptor
{
    public string Encrypt(string key, string text);
    public string Decrypt(string key, string text);
}