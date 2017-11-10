using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimWaterfall
{
	public class Gameboard
	{
		#region Properties

		public int[] Heaps { get; set; }
		public string P1Name { get; set; } = "Player 1";
		public string P2Name { get; set; } = "CPU";
		public bool P2IsHuman { get; set; } = false;
		public bool P1Turn { get; set; } = true;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a gameboard based on the given difficulty.
		/// </summary>
		/// <param name="difficulty">The difficulty determines how many heaps there are, and how many beads in each heap.</param>
		public Gameboard(Difficulty difficulty)
		{
			switch(difficulty)
			{
				//Set heap size and count based on difficulty.
				case Difficulty.Easy:
					Heaps = new int[2];
					Heaps[0] = 3;
					Heaps[1] = 3;
					break;
				case Difficulty.Medium:
					Heaps = new int[3];
					Heaps[0] = 2;
					Heaps[1] = 5;
					Heaps[2] = 7;
					break;
				case Difficulty.Hard:
					Heaps = new int[4];
					Heaps[0] = 2;
					Heaps[1] = 3;
					Heaps[2] = 8;
					Heaps[3] = 9;
					break;
			}

			//Determine who goes first.
			P1Turn = new Random().Next(1, 3) % 2 == 0;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Allows the computer to make a random, legal, non-suicidal move.
		/// </summary>
		public void AITakeTurn()
		{
			Random rando = new Random();

			if (Heaps.Sum() > 1) //Do not intentionally lose.
			{
				bool validHeap = false;
				int heap = 0;
				int beads = 0;
				while (!validHeap)
				{
					heap = rando.Next(Heaps.Length);

					if (Heaps[heap] != 0)
						validHeap = true;
				}

				bool validBeads = false;
				while (!validBeads)
				{
					beads = rando.Next(Heaps[heap])+1;
					if (Heaps.Where(x => x > 0).Count() == 1 && beads == Heaps.Sum())
						validBeads = false;
					else
						validBeads = true;
				}

				Heaps[heap] -= beads;

				Console.Clear();
				Console.WriteLine($"{P2Name} removed {beads} from heap {heap+1}.");

			}
			else //The only available move is to lose.
			{
				for(int i = 0; i < Heaps.Length; i++)
				{
					if (Heaps[i]==1)
					{
						Heaps[i] -= 1;
						Console.Clear();
						Console.WriteLine($"{P2Name} removed 1 from heap {i+1}");
						break;
					}
				}
			}

		}

#endregion

		#region Overrides

		/// <summary>
		/// Compiles the current state of the board into one string.
		/// </summary>
		/// <returns>The current state of the board in human-readable format. </returns>
		public override string ToString()
		{
			StringBuilder retVal = new StringBuilder();

			for (int i = 0; i < Heaps.Length; i++)
			{
				retVal.Append($"Heap {i+1} has {Heaps[i]} beads \n");
			}

			return retVal.ToString();
		}

		#endregion
	}
}
