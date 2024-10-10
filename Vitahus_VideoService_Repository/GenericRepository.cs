using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Vitahus_VideoService_Repository;

public class GenericRepository<T>(IMongoDatabase database, ILogger<GenericRepository<T>> logger)
    : IGenericRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection = database.GetCollection<T>(
        typeof(T).Name + "s"
    );


    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting all entities from the collection\n");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "GetAllAsync took {ElapsedMilliseconds} ms \n",
                stopwatch.ElapsedMilliseconds
            );
        }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var stopwatch = Stopwatch.StartNew();
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id), "Id is empty - cannot get entity\n");
        }

        try
        {
            return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting entity from the collection\n");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "GetByIdAsync took {ElapsedMilliseconds} ms \n",
                stopwatch.ElapsedMilliseconds
            );
        }
    }

    public async Task? CreateAsync(T entity)
    {
        var stopwatch = Stopwatch.StartNew();
        if (entity == null)
        {
            logger.LogError($"Entity is null - cannot create {nameof(entity)}\n");
            throw new ArgumentNullException(
                nameof(entity),
                $"{nameof(entity)} is null - cannot create {nameof(entity)}\n"
            );
        }

        try
        {
            await _collection.InsertOneAsync(entity);
        }
        catch (MongoWriteException e)
        {
            logger.LogError(e, "Error writing entity to the collection\n");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "CreateAsync took {ElapsedMilliseconds} ms \n",
                stopwatch.ElapsedMilliseconds
            );
        }
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        var stopwatch = Stopwatch.StartNew();
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id), "Id is empty - cannot update entity\n");
        }

        if (entity == null)
        {
            throw new ArgumentNullException(
                nameof(entity),
                $"Entity is null - cannot update {nameof(entity)}\n"
            );
        }

        try
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating entity in the collection\n");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "UpdateAsync took {ElapsedMilliseconds} ms \n",
                stopwatch.ElapsedMilliseconds
            );
        }
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var stopwatch = Stopwatch.StartNew();
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id), "Id is empty - cannot delete entity\n");
        }

        try
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting entity from the collection\n");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "DeleteAsync took {ElapsedMilliseconds} ms \n",
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}