namespace NetScape.Abstractions.Util
{
    public class TextUtil
    {
		/// <summary>
		/// An array of characters ordered by frequency - the elements with lower indices (generally) appear more often in
		/// chat messages.
		/// </summary>
		private static char[] VALID_CHARACTERS = { '_', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
			'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6',
			'7', '8', '9' };

		public static long NameToLong(string name)
		{
			long longName = 0L;
			for (int i = 0; i < name.Length && i < 12; i++)
			{
				char c = name[i];
				longName *= 37L;
				if (c >= 'A' && c <= 'Z')
				{
					longName += (1 + c) - 65;
				}
				else if (c >= 'a' && c <= 'z')
				{
					longName += (1 + c) - 97;
				}
				else if (c >= '0' && c <= '9')
				{
					longName += (27 + c) - 48;
				}
			}
			while (longName % 37L == 0L && longName != 0L)
			{
				longName /= 37L;
			}
			return longName;
		}

		public static string LongToName(long longName)
		{
			if (longName <= 0L || longName >= 6582952005840035281L)
			{
				return "invalid_name";
			}
			if (longName % 37L == 0L)
			{
				return "invalid_name";
			}
			int length = 0;
			var name = new char[12];
			while (longName != 0L)
			{
				long l1 = longName;
				longName /= 37L;
				name[11 - length++] = VALID_CHARACTERS[(int)(l1 - longName * 37L)];
			}
			return new string(name, 12 - length, length);
		}
	}
}
