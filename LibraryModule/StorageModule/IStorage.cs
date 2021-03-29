﻿using System.Collections.Generic;

namespace LibraryModule
{
    public interface IStorage
    {
        void Add(string key, string value);

        string GetValue(string key);

        void AddInSet(string newKey, string value);

        bool ExistInSet(string newKey, string value);
    }
}