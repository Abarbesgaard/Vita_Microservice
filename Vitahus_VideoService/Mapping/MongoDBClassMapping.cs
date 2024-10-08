using System.Diagnostics;
using MongoDB.Bson.Serialization;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService.Mapping;

public static class MongoDbClassMapping
{
    public static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Video)))
        {
            BsonClassMap.RegisterClassMap<Video>(cm =>
            {
                cm.AutoMap();  // Auto-maps all public properties.
                cm.SetIgnoreExtraElements(true);  // Ignores any extra fields not mapped in the class.
            });
        }
        if (!BsonClassMap.IsClassMapRegistered(typeof(Activity)))
        {
            BsonClassMap.RegisterClassMap<Activity>(cm =>
            {
                cm.AutoMap();  // Auto-maps all public properties.
                cm.SetIgnoreExtraElements(true);  // Ignores any extra fields not mapped in the class.
            });
        } 
        if (!BsonClassMap.IsClassMapRegistered(typeof(AuditLog)))
        {
            BsonClassMap.RegisterClassMap<AuditLog>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        } 
    } 
}