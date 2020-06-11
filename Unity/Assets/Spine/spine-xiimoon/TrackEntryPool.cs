using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine
{
    public static class TrackEntryPool
    {
        private static int s_id = 0;
        private static readonly ObjectPool<TrackEntry> s_listEntry = new ObjectPool<TrackEntry>(OnCreateFunc, OnReleaseFunc);

        public static int GenerateID()
        {
            return s_id++;
        }

        private static void OnCreateFunc(TrackEntry entry)
        {
            //XiimoonLog.LogFormat("+++++ TrackEntryPool Create : {0}, maxCount : {1}", entry.EntryID, s_id);
        }

        private static void OnReleaseFunc(TrackEntry entry)
        {
            //XiimoonLog.LogFormat("---- TrackEntryPool Release : {0}, maxCount : {1}", entry.EntryID, s_id);
            entry.Reset();
        }

        public static TrackEntry Get()
        {
            return s_listEntry.Get();
        }
        
        public static void Recycle(TrackEntry entry)
        {
            s_listEntry.Release(entry);
        }
        
    }
}