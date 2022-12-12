using System.Collections.Generic;
using System.Linq;
using WPFCalculator.Models;

namespace WPFCalculator.Helpers
{
    public static class MemoryStorageHelper
    {
        public static void ClearMemory(List<MemoryHistory> memorylist)
        {
            memorylist.Clear();
        }

        public static List<MemoryHistory> ClearMemoryItem(List<MemoryHistory> MemoryList, MemoryHistory memoryItem)
        {
             MemoryList.Remove(memoryItem);
            return MemoryList;
        }



    }
}
