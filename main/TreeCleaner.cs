using System.Collections.Generic;
using Eto.Parse;

public class TreeCleaner
{
	Stack<int> indexesToRemove = new Stack<int>();
	HashSet<int> positions = new HashSet<int>();

	public void RemoveExtraNodes(Match match)
	{
		for (int i = 0; i < match.Matches.Count; ++i)
		{
			var node = match.Matches[i];
			if (positions.Contains(node.Index))
			{
				indexesToRemove.Push(i);
			}
			else
			{
				positions.Add(node.Index);
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
