using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSC160_ConsoleMenu;

namespace NimWaterfall
{
	public static class Engine
	{

		static Gameboard board;

		/// <summary>
		/// The main program loop, which encompasses all functionality in the application.
		/// </summary>
		public static void MainLoop()
		{
			bool quit = false;

			while (!quit)
			{
				//Greet the user!
				Console.WriteLine("Welcome to Nim!");

				//Send the user to the main menu.
				MainMenu();

				//Play the game
				Play();

				//Ask if the user would like to play again.
				quit = !CIO.PromptForBool("Play again?", "Y", "N");

				if (quit)
					Console.WriteLine("Goodbye");
			}
		}
		
		/// <summary>
		/// The main menu, which recieves user input about game options like difficulty and game mode.
		/// </summary>
		public static void MainMenu()
		{
			//Prompt the user for their game mode (Choices are PvP and PvE).
			bool PvP = CIO.PromptForBool("Please choose your game mode: ", "PvP", "PvE", true);

			//Clear console for readability
			Console.Clear();

			//Prompt the user for their prefered difficulty
			Difficulty difficulty;
			switch(CIO.PromptForMenuSelection(new string[] { "Easy", "Medium", "Hard" }, false ))
			{
				case 1:
					difficulty = Difficulty.Easy;
					break;
				case 2:
					difficulty = Difficulty.Medium;
					break;
				default:
					difficulty = Difficulty.Hard;
					break;
			}

			//Clear console for readability
			Console.Clear();

			//Prompt player 1 for their name.
			string p1name = CIO.PromptForInput("Player 1, please enter your name: ", false, true);

			//Clear console for readability
			Console.Clear();

			//If the game mode is PvP, also prompt player 2 for their name.
			string p2Name = "CPU";
			if (PvP)
				p2Name = CIO.PromptForInput("Player 2, please enter your name: ", false, true);

			//Clear console for readability
			Console.Clear();

			//Store all values and end the main menu.
			board = new Gameboard(difficulty);
			board.P1Name = p1name;
			board.P2IsHuman = PvP;
			if (PvP)
				board.P2Name = p2Name;
		}

		/// <summary>
		/// The game loop which handles turn assignment and win-condition checking.
		/// </summary>
		public static void Play()
		{
			bool gameOver = false;

			while (!gameOver)
			{
				//Print out the current state of the game board, and who's turn it is.
				if (board.P1Turn)
					Console.WriteLine($"\n{board.P1Name}'s Turn");
				else
					Console.WriteLine($"{board.P2Name}'s Turn");
				Console.WriteLine("\n" + board + "\n");
				Console.WriteLine("Enter \"h\" at any time for the rules, or \"x\" at any time to forfeit the match, and return to the menu.\n");

				gameOver = !TakeTurn(); //The current player takes their turn. If TakeTurn returns false, set gameOver to true.

				if (!gameOver)
					if (!board.Heaps.Any(h => h > 0)) //Check if any heaps have more than 0 beads remaining
						gameOver = true; //If none have any beads remaining, end the game.
					else //If there is at least one bead remaining on at least one heap, go to the next turn, and continue the loop.
						board.P1Turn = !board.P1Turn;
			}

			//Declare a winner- the player who's turn it was upon the game's end is the loser.
			if (board.P1Turn)
			{
				Console.WriteLine($"{board.P2Name} wins!");
			}
			else
			{
				Console.WriteLine($"{board.P1Name} wins!");
			}
		}

		/// <summary>
		/// The method which recieves input from the player who's turn it is
		/// </summary>
		/// <returns>True if the game should continue, false if the game should end.</returns>
		public static bool TakeTurn()
		{
			if (board.P1Turn) //Player 1 is always human, so if this is true, we get user input.
			{
				bool validInput = false;
				int heap = 0;
				int beads = 0;

				//Determine empty heaps.
				List<int> emptyHeaps = new List<int>(0);
				for (int i = 0; i<board.Heaps.Length; i++)
				{
					if (board.Heaps[i] < 1)
						emptyHeaps.Add(i+1);
				}

				//Loop while a heap and bead-count have not bee properly selected.
				while (!validInput)
				{
					//Prompt the user for which heap they'd like to remove beads from.
					heap = PromptForUserChoice("Please select a heap to remove beads from: ", 1, board.Heaps.Length, emptyHeaps.ToArray()) -1;

					//If the method returned 0, exit.
					if (heap == -1)
						return false;

					//Prompt the user for the amount of beads they wish to remove.
					beads = PromptForUserChoice($"Please enter the number of beads you wish to remove from heap {heap + 1}: ", 1, board.Heaps[heap], new int[] { -1 });

					//If The method returned 0, exit.
					if (beads == 0)
						return false;

					validInput = true;
				}

				//Decrement the beads in th especified heap by the user's number.
				board.Heaps[heap] -= beads;

				//Clear console for readability
				Console.Clear();

				//Print out the player's action.
				Console.WriteLine($"{board.P1Name} removed {beads} from heap {heap+1}");

				//return true;
				return true;
			}
			else //If it is not player 1's turn, we need to check if player 2 is human
			{
				if (board.P2IsHuman) //If player 2 is human, prompt the user for their input.
				{
					bool validInput = false;
					int heap = 0;
					int beads = 0;

					//Determine empty heaps.
					List<int> emptyHeaps = new List<int>(0);
					for (int i = 0; i < board.Heaps.Length; i++)
					{
						if (board.Heaps[i] < 1)
							emptyHeaps.Add(i);
					}

					//Loop while a heap and bead-count have not bee properly selected.
					while (!validInput)
					{
						//Prompt the user for which heap they'd like to remove beads from.
						heap = PromptForUserChoice("Please select a heap to remove beads from: ", 1, board.Heaps.Length, emptyHeaps.ToArray()) - 1;

						//If the method returned 0, exit.
						if (heap == -1)
							return false;

						//Prompt the user for the amount of beads they wish to remove.
						beads = PromptForUserChoice($"Please enter the number of beads you wish to remove from heap {heap + 1}: ", 1, board.Heaps[heap], new int[] { -1 });

						//If The method returned 0, exit.
						if (beads == 0)
							return false;

						validInput = true;
					}

					//Decrement the beads in th especified heap by the user's number.
					board.Heaps[heap] -= beads;

					//Clear console for readability
					Console.Clear();

					//Print out the player's action.
					Console.WriteLine($"{board.P2Name} removed {beads} from heap {heap + 1}");

					//return true;
					return true;
				}
				else //If player 2 is a bot, redirect to board.AITakeTurn()
				{
					board.AITakeTurn();
					return true;
				}
			}
		}

		/// <summary>
		/// A method for prompting the user for a number, while also accepting "h" and "x" as valid input.
		/// </summary>
		/// <param name="promptMessage">The message that will be displayed to the user</param>
		/// <param name="min">The lowest available choice.</param>
		/// <param name="max">The highest available choice.</param>
		/// <param name="invalidOptions">Any options the user is not allowed to use (empty heaps).</param>
		/// <returns>Returns a number greater than 0 if the user selects valid choices, returns 0 if the user wishes to exit</returns>
		public static int PromptForUserChoice(string promptMessage, int min, int max, int[] invalidOptions)
		{
			bool validInput = false;
			int retVal = 0;

			//Loop while user has not entered a valid number.
			while (!validInput)
			{
				string userInput = CIO.PromptForInput(promptMessage, false, true);

				if (userInput.Trim().Equals("h", StringComparison.CurrentCultureIgnoreCase))
				{
					PrintRules();
				}
				else if (userInput.Trim().Equals("x", StringComparison.CurrentCultureIgnoreCase))
				{
					return 0;
				}
				else
				{
					int i;
					if (int.TryParse(userInput, out i)) //Check if the user entered a valid integer
					{
						if (i < min)
						{
							Console.WriteLine("Your choice was too low.");
						}
						else if (i>max)
						{
							Console.WriteLine("Your choice was too high.");
						}
						else if (invalidOptions.Any(x => x==i))
						{
							Console.WriteLine("That option is invalid.");
						}
						else
						{
							retVal = i;
							validInput = true;
						}
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Prints the rules of Nim to the console. 
		/// </summary>
		public static void PrintRules()
		{
			Console.WriteLine("The game of Nim consists of several heaps, each of which contains a number of beads. Each player takes turns removing at least one bead, but up to all of the beads on a given heap- however, a player may only remove beads from one heap per turn. The player who removes the last bead across all heaps loses.");
		}
	}
}
