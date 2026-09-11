namespace DDoveFramework.Editor
{
    public static class DDoveEditorNav
    {
        public static readonly string[] Ids =
        {
            "hotbox",
            "architecture",
            "res",
            "ui",
        };

        public static int IndexOf(string id)
        {
            for (var i = 0; i < Ids.Length; i++)
            {
                if (Ids[i] == id)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
