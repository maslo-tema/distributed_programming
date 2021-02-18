using System.Collections.Generic;

namespace Valuator
{
    public interface IStorage
    {
        void Add(string key, string value);

        string GetValue(string key);

        bool Exist(string prefix, string value);
    }
}