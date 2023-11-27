using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNegativeCountException : Exception
{
    public ItemNegativeCountException() : base("Item count cannot be negative.")
    {
    }

    public ItemNegativeCountException(string message) : base(message)
    {
    }

    public ItemNegativeCountException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
