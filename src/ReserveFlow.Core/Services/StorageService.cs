using System.IO;
using System.Threading.Tasks;
using SQLite;
using ReserveFlow.Core.Models;

namespace ReserveFlow.Core.Services;

public class StorageService : IStorageService
{
    private SQLiteAsyncConnection? _db;

    public async Task InitializeAsync()
    {
        if (_db != null)
            return;

        var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "reserveflow.db");
        _db = new SQLiteAsyncConnection(dbPath);
        await _db.CreateTableAsync<GeotaggedAudioAttachment>();
    }

    public async Task SaveAttachmentAsync(GeotaggedAudioAttachment attachment)
    {
        if (_db == null)
            await InitializeAsync();

        await _db!.InsertAsync(attachment);
    }
}
