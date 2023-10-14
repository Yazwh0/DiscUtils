//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace BitMagic.DiscUtils.Internal;

/// <summary>
/// Caches objects.
/// </summary>
/// <typeparam name="K">The type of the object key.</typeparam>
/// <typeparam name="V">The type of the objects to cache.</typeparam>
/// <remarks>
/// Can be use for two purposes - to ensure there is only one instance of a given object,
/// and to prevent the need to recreate objects that are expensive to create.
/// </remarks>
internal class ObjectCache<K, V> where V : class
{
    private const int MostRecentListSize = 20;
    private const int PruneGap = 500;

    private readonly Dictionary<K, WeakReference<V>> _entries;
    private int _nextPruneCount;
    private readonly List<KeyValuePair<K, V>> _recent;

    public ObjectCache()
    {
        _entries = new Dictionary<K, WeakReference<V>>();
        _recent = new List<KeyValuePair<K, V>>();
    }

    public ObjectCache(IEqualityComparer<K> comparer)
    {
        _entries = new Dictionary<K, WeakReference<V>>(comparer);
        _recent = new List<KeyValuePair<K, V>>();
    }

    public V this[K key]
    {
        get
        {
            for (var i = 0; i < _recent.Count; ++i)
            {
                var recentEntry = _recent[i];
                if (recentEntry.Key.Equals(key))
                {
                    MakeMostRecent(i);
                    return recentEntry.Value;
                }
            }

            if (_entries.TryGetValue(key, out var wRef))
            {
                if (wRef.TryGetTarget(out var val))
                {
                    MakeMostRecent(key, val);
                }

                return val;
            }

            return default(V);
        }

        set
        {
            _entries[key] = new WeakReference<V>(value);
            MakeMostRecent(key, value);
            PruneEntries();
        }
    }

    internal void Remove(K key)
    {
        for (var i = 0; i < _recent.Count; ++i)
        {
            if (_recent[i].Key.Equals(key))
            {
                _recent.RemoveAt(i);
                break;
            }
        }

        _entries.Remove(key);
    }

    private void PruneEntries()
    {
        _nextPruneCount++;

        if (_nextPruneCount > PruneGap)
        {
            var toPrune = new List<K>();
            foreach (var entry in _entries)
            {
                if (!entry.Value.TryGetTarget(out _))
                {
                    toPrune.Add(entry.Key);
                }
            }

            foreach (var key in toPrune)
            {
                _entries.Remove(key);
            }

            _nextPruneCount = 0;
        }
    }

    private void MakeMostRecent(int i)
    {
        if (i == 0)
        {
            return;
        }

        var entry = _recent[i];
        _recent.RemoveAt(i);
        _recent.Insert(0, entry);
    }

    private void MakeMostRecent(K key, V val)
    {
        while (_recent.Count >= MostRecentListSize)
        {
            _recent.RemoveAt(_recent.Count - 1);
        }

        _recent.Insert(0, new KeyValuePair<K, V>(key, val));
    }
}