using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTypeException : Exception
{
    public ItemTypeException() : base("Invalid item type.")
    {
    }

    public ItemTypeException(string message) : base(message)
    {
    }

    public ItemTypeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
