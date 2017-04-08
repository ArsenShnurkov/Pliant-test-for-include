using System.Collections.Generic;
using Eto.Parse;

public class TreeCleaner
{
	HashSet<KeyValuePair<int, int>> positions = new HashSet<KeyValuePair<int, int>>();

	public void RemoveExtraNodes(Match match)
	{
		Stack<int> indexesToRemove = new Stack<int>();
		for (int i = 0; i < match.Matches.Count; ++i)
		{
			var node = match.Matches[i];
			bool bDuplicate = positions.Contains(new KeyValuePair<int,int>(node.Index, node.Length));
			if (bDuplicate)
			{
				indexesToRemove.Push(i);
			}
			else
			{
				positions.Add(new KeyValuePair<int, int>(match.Index, match.Length));
			}
			RemoveExtraNodes(node);
		}
		while (indexesToRemove.Count > 0)
		{
			int i = indexesToRemove.Pop();
			match.Matches.RemoveAt(i);
		}
	}

}
