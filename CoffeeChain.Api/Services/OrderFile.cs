using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CoffeeChain.Api.Contracts.Orders;

namespace CoffeeChain.Api.Services;

internal sealed class OrderFile
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    private readonly string _path;
    private readonly string _mutexName;

    public OrderFile(string? path)
    {
        _path = Path.GetFullPath(string.IsNullOrWhiteSpace(path)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoffeeChain", "orders.json")
            : path);
        var mutexKey = OperatingSystem.IsWindows() ? _path.ToUpperInvariant() : _path;
        _mutexName = "CoffeeChain.Orders." + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(mutexKey)));
    }

    public T Read<T>(Func<List<OrderDto>, T> read) => Access(read, save: false);

    public T Update<T>(Func<List<OrderDto>, T> update) => Access(update, save: true);

    private T Access<T>(Func<List<OrderDto>, T> action, bool save)
    {
        using var mutex = new Mutex(false, _mutexName);
        try
        {
            mutex.WaitOne();
        }
        catch (AbandonedMutexException)
        {
            // The previous process stopped while writing. The committed file is still intact.
        }

        try
        {
            var orders = Load();
            var result = action(orders);
            if (save) Save(orders);
            return result;
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private List<OrderDto> Load()
    {
        try
        {
            using var input = File.OpenRead(_path);
            return JsonSerializer.Deserialize<List<OrderDto>>(input, JsonOptions)
                ?? throw new JsonException("Tệp đơn hàng không chứa danh sách đơn.");
        }
        catch (FileNotFoundException)
        {
            return new List<OrderDto>();
        }
        catch (DirectoryNotFoundException)
        {
            return new List<OrderDto>();
        }
    }

    private void Save(List<OrderDto> orders)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var tempPath = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var output = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(output, orders, JsonOptions);
                output.Flush(flushToDisk: true);
            }
            File.Move(tempPath, _path, overwrite: true);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
