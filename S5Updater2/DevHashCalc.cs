namespace S5Updater2
{
    internal static class DevHashCalc
    {
        internal static uint CalcHash(string n)
        {
            if (string.IsNullOrEmpty(n))
                return 0;
            uint hash = 0;
            n = n.ToLower();
            foreach (var c in n)
            {
                hash = c + (hash << 4);
                if ((hash & 0xf0000000) != 0)
                {
                    hash = (hash ^ hash >> 24 & 0xf0) & 0xfffffff;
                }
            }
            if (hash == 0)
                return 0;
            return ((hash >> 16) * 0x6c078965 - hash * 0x769b0000) * 0x44169C2F;
        }

    }
}
