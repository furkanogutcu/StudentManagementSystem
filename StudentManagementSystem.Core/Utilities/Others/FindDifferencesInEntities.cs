using System;
using System.Collections.Generic;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Core.Utilities.Others
{
    public static class FindDifferencesInEntities
    {
        public static Dictionary<string, bool> Find<T>(T firstEntity, T secondEntity)
            where T : class, IEntity, new()
        {
            var entitiesProperties = firstEntity.GetType().GetProperties();

            Dictionary<string, bool> returnDictionary = new Dictionary<string, bool>();

            foreach (var propertyInfo in entitiesProperties)
            {
                if (propertyInfo.PropertyType == typeof(DateTime?))
                {
                    if (propertyInfo.GetValue(firstEntity) == null && propertyInfo.GetValue(secondEntity) == null)
                    {
                        returnDictionary.Add(propertyInfo.Name, false);
                    }
                    else
                    {
                        returnDictionary.Add(propertyInfo.Name, ((DateTime)propertyInfo.GetValue(firstEntity)).Date != ((DateTime)propertyInfo.GetValue(secondEntity)).Date);
                    }
                }
                else
                {
                    returnDictionary.Add(propertyInfo.Name, !propertyInfo.GetValue(firstEntity).Equals(propertyInfo.GetValue(secondEntity)));
                }
            }

            return returnDictionary;
        }
    }
}
