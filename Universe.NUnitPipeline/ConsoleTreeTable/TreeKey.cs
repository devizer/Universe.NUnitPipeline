using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline.ConsoleTreeTable
{
	// Immutable Array Of Strings
	public class TreeKey
	{
		private readonly Lazy<int> _HashCode;
		private readonly Lazy<string> _ToString;
		public string[] Path { get; }

		public TreeKey()
		{
			_ToString = new Lazy<string>(() =>
			{
				// const string arrow = " \x27a1 ";
				const string arrow = " \x2192 ";
				return string.Join(arrow, Path ?? new string[0]);
			}, LazyThreadSafetyMode.ExecutionAndPublication);

			_HashCode = new Lazy<int>(() =>
			{
				if (Path == null) return 0;
				int ret = 0;
				unchecked
				{
					foreach (var p in Path)
						ret = ret * 397 ^ (p?.GetHashCode() ?? 0);
				}

				return ret;

			}, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public TreeKey(params string[] path) : this()
		{
			Path = path ?? throw new ArgumentNullException(nameof(path));
			for (int i = 0, l = path.Length; i < l; i++)
				if (path[i] == null)
					throw new ArgumentException($"path's element #{i} is null", nameof(path));
		}

		public TreeKey Child(string childName)
		{
			if (childName == null) throw new ArgumentNullException(nameof(childName));
			return new TreeKey(Path.Concat(new[] { childName }).ToArray());
		}

		public override string ToString() => _ToString.Value;

		protected bool Equals(TreeKey other)
		{
			if (_HashCode.Value != other._HashCode.Value) return false;

			var len = Path.Length;
			var lenOther = other.Path.Length;
			if (len != lenOther) return false;
			for (int i = 0; i < len; i++)
				if (!Path[i].Equals(other.Path[i], StringComparison.InvariantCultureIgnoreCase))
					return false;

			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TreeKey)obj);
		}

		public override int GetHashCode()
		{
			return _HashCode.Value;
		}

		class TempTree : Dictionary<TreeKey, TempTree>
		{
			// null for sub tree
			public TreeKey Leaf;
		}

		public static List<Node<TreeKey>> AsTree(IEnumerable<TreeKey> plainList)
		{
			TempTree root = new TempTree();
			foreach (var plainItem in plainList)
			{
				var parent = root;
				for (int i = 0, l = plainItem.Path.Length; i < l; i++)
				{
					TreeKey partial = new TreeKey(plainItem.Path.Take(i + 1).ToArray());
					TempTree current = parent.GetOrAdd(partial, key => new TempTree());
					parent = current;
				}
			}

			if (plainList.Count() >= 33)
			{
				var breakHere = "ok";
			}


			List<Node<TreeKey>> ret = new List<Node<TreeKey>>();
			EnumSubTree(root, ret);
			return ret;
		}

		private static void EnumSubTree(TempTree treeNode, List<Node<TreeKey>> nodes)
		{

			foreach (var pair in treeNode)
			{
				TreeKey key = pair.Key;
				TempTree subTree = pair.Value;
				Node<TreeKey> subNode = new Node<TreeKey>()
				{
					State = key,
					Name = key.Path.Last(),
				};
				nodes.Add(subNode);
				EnumSubTree(subTree, subNode.Children);
			}

			var sorted = nodes.OrderBy(x => x.Name).ToList();
			nodes.Clear();
			nodes.AddRange(sorted);
		}
	}
}
