using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "ContainerDatabase", menuName = "Database/Container Database")]
public class ContainerDatabase : ScriptableObject
{
    public List<ContainerDatabaseEntity> allContainers;

    public ContainerDatabaseEntity GetByName(string name)
    {
        return allContainers.FirstOrDefault(containers => containers.Name == name);
    }

    public ContainerDatabaseEntity GetRandomContainer(ContainerType type)
    {
        var filteredContainers = allContainers.FindAll(container => container.Type == type);
    
        if (filteredContainers.Count == 0)
        {
            return null;
        }

        var random = new Random();
        var randomIndex = random.Next(filteredContainers.Count);

        return filteredContainers[randomIndex];
    }
}

[Serializable]
public class ContainerDatabaseEntity
{
    public ContainerType Type;
    public string Name;
    private string Path = @"Containers\";

    public ContainerDatabaseEntity(ContainerType type, string name)
    {
        Type = type;
        Name = name;
    }
    
    public override string ToString()
    {
        return GetAssetBundleName() + "\\" + Name;
    }

    public string GetAssetBundleName()
    {
        return Path + Type;
    }
}