using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PokerOddsCalculator
{
	//Calculates probabilities of various hands by running many simulations
	class OddsCalculator
	{
		private const int NUM_WORKERS = 8;

		//Number of times each poker hand came up in ascending order of value, 
		// (so, high card > pair > two pair > three of a kind etc. 
		//Dividing each index of this array by _SimulationsRan and multiplying 
		//	by 100 gets the probability for each hand as a percentage
		private int[] _Events = new int[10]; 
		private int _SimulationsRan = 0;
		private Thread[] _Threads = new Thread[NUM_WORKERS];
		private Barrier _SimulationsDone = new Barrier(NUM_WORKERS + 1); //all workers + main thread
		private Object _Locker = new Object();

		private int FindNumberOfUnknownCards(Card[] cards)
		{
			int unknownCards = 0;
			foreach (Card s in cards)
			{
				if (!s.IsKnown)
					unknownCards++;
			}
			return unknownCards;
		}

		private void SortCards(ref Card[] hand, int unknownCards)
		{	//Sort cards array with all unknown cards at the back to 
			//make dealing new cards easier and speed up simulations
			for (int i = 0; i < unknownCards; i++)
			{
				for (int u = 0; u < hand.Length; u++)
				{
					if (!hand[u].IsKnown)
					{
						Card temp = hand[hand.Length - 1 - i];
						hand[hand.Length - 1 - i] = hand[u];
						hand[u] = temp;
						break;
					}
				}
			}
		}

		private float[] GetDefaultProbabilities()
		{	//Default probabilities from Wikipedia: http://en.wikipedia.org/wiki/Poker_probability
			//It's important to note that the probabilities are for that exact hand, not that hand or better
			float[] probabilities = new float[10];
			probabilities[0] = 17.4f; //High Card
			probabilities[1] = 43.8f; //Pair
			probabilities[2] = 23.5f; //Two Pair
			probabilities[3] = 4.83f; //Three of a Kind
			probabilities[4] = 4.62f; //Straight
			probabilities[5] = 3.03f; //Flush
			probabilities[6] = 2.60f; //Full House
			probabilities[7] = 0.168f;//Four of a Kind
			probabilities[8] = 0.0279f;//Straight Flush
			probabilities[9] = 0.0032f;//Royal Flush
			return probabilities;
		}

		/// <summary>
		/// Returns likelihood of getting each possible hand as an array
		/// Note that "cards" includes both the board and hand
		/// </summary>
		public float[] RunSimulations(List<Card> cards, int simulationsToRun)
		{
			if (cards.Count > 7)
				throw new ArgumentException("Too many cards!");
			while (cards.Count < 7)
				cards.Add(new Card() { IsKnown = false });
			var cardsArray = cards.ToArray();

			int unknownCards = FindNumberOfUnknownCards(cardsArray);

			//Return if no cards have been defined (use precalculated probabilities)
			if (unknownCards == 7)
			{
				_SimulationsRan = -1; //allows others to know when using lookup table
				return GetDefaultProbabilities();
			}

			float[] probabilities = new float[10]; //probability array to be returned

			//Return if all cards have already been defined
			if (unknownCards == 0)
			{
				probabilities[(int)(new PokerHandEvaluator()).Evaluate(cardsArray)] = 100;
				return probabilities; 
			}

			SortCards(ref cardsArray, unknownCards);

			//Run actual simulations
			for (int i = 0; i < NUM_WORKERS; i++)
			{
				_Threads[i] = new Thread(() => Simulate(simulationsToRun/NUM_WORKERS, unknownCards, cardsArray));
				_Threads[i].Start();
			}

			_SimulationsDone.SignalAndWait(); //all simulations will be done before passing this point
			_SimulationsDone.AddParticipants(NUM_WORKERS); //reset barrier

			//Return in probability array form, where each index is the probability of a hand
			for (int i = 0; i < _Events.Length; i++)
				probabilities[i] = _Events[i] / (float)_SimulationsRan * 100;
			return probabilities;
		}

		//The method each thread runs
		private void Simulate(int simulationsToRun, int unknownCards, Card[] cardsArray)
		{
			PokerHandEvaluator handEvaluator = new PokerHandEvaluator();
			Random rand = RandomHelper.Instance; //ensures each thread gets a unique seed :)
			Card temp;
			Card[] cards = new Card[7]; //copy of array, avoids locking of cardsArray
			for (int i = 0; i < 7; i++)
				cards[i] = cardsArray[i];

			//Run simulations
			for (int i = 0; i < simulationsToRun; i++)
			{
				//Deal random cards
				for (int u = 0; u < unknownCards; u++)
				{
					temp = new Card((Rank)rand.Next(13) + 1, (Suit)rand.Next(4));
					if (cards.Contains(temp))
						u--;
					else
						cards[cards.Length - 1 - u] = temp;
				}

				//Work out hand value
				PokerHand value = handEvaluator.Evaluate(cards);
				lock (_Locker)
				{
					_Events[(int)value]++;
					_SimulationsRan++;
				}

				//Clear dealt cards (otherwise it won't deal them in the next iteration, distorting the calculation)
				for (int u = cards.Length - 1; u > cards.Length - 1 - unknownCards; u--)
					cards[u].IsKnown = false;
			}

			_SimulationsDone.RemoveParticipant(); //like SignalAndWait but doesn't wait (no need)
		}

		public bool UsingLookupTable
		{
			get { return _SimulationsRan == -1; }
		}

		public int SimulationsRan
		{
			get { return UsingLookupTable ? 0 : _SimulationsRan; }
		}
	}
}