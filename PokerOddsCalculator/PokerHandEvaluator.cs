using System;

namespace PokerOddsCalculator
{
	public enum PokerHand
	{
		HighCard,
		Pair,
		TwoPair,
		ThreeOfAKind,
		Straight,
		Flush,
		FullHouse,
		FourOfAKind,
		StraightFlush,
		RoyalFlush
	}

	class PokerHandEvaluator
	{
		//Stores number of each card in the hand
		//where 0 is an ace, 12 is a king (integer cast of rank - 1)
		private int[] _CardRankHistogram = new int[13]; 

		//Evaluation is designed to work as a tree.
		//	             HighCard
		//	            /   |    \
		//	           /    |     \
		//	          /      \     \
		//	       Pair     Flush  Straight
		//	       /| \          \   /
		//	      / |  \      StraightFlush
		//	     /  |   \             \
		//  TwoPair | ThreeOfAKind     \
		//          |   /    \        RoyalFlush
		//	        |  /      \
		//	   FullHouse  FourOfAKind
		//
		//As you can see, it's impossible to have a straight or a flush if you have a pair, for example. This simplifies evaluation quite a bit.
		//Really TwoPair and ThreeOfAKind are simply special cases of Pair, and a FullHouse is simply a Pair and a ThreeOfAKind together.
		//Every time a hand is found it is written to result, then higher-value hands are checked which overwrite result if necessary.
		//This isn't the most efficient way of doing it, but it is very terse and easy to understand in code-form.
		//
		//Note that the current implementation will simply call two equal PokerHand values a tie, without consideration of the high card. 
		//For example a 2-6 straight is equal to a 10-Ace straight.
		private PokerHand EvaluateFiveCards(Card[] hand)
		{
			PokerHand result = PokerHand.HighCard;
			UpdateHistogram(hand);

			int numPairs = CountPairs();
			if (numPairs > 0)
			{
				result = numPairs == 1 ? PokerHand.Pair : PokerHand.TwoPair;
				
				int numEqualRanks = CountEqualRanks();
				if (numEqualRanks == 3)
					result = numPairs == 2 ? PokerHand.FullHouse : PokerHand.ThreeOfAKind; //Three of a kind is also considered a pair
				else if (numEqualRanks == 4)
					result = PokerHand.FourOfAKind;
			}
			else
			{
				bool isFlush = IsFlush(hand);
				bool isStraight = IsStraight();

				if (isFlush && isStraight)
				{
					if (GetHighCard() == Rank.Ace && _CardRankHistogram[12] == 1) //if (straight includes ace && straight includes king)... otherwise ace-5 = royal flush
						result = PokerHand.RoyalFlush;
					else result = PokerHand.StraightFlush;
				}
				else if (isFlush)
					result = PokerHand.Flush;
				else if (isStraight)
					result = PokerHand.Straight;
			}
			return result;
		}

		private class RecursiveSevenCardEvaluator
		{
			PokerHandEvaluator outer;
			PokerHand _BestHand = PokerHand.HighCard;
			bool[] _UsedCardIdx = new bool[7];

			public RecursiveSevenCardEvaluator(PokerHandEvaluator outerClass)
			{
				outer = outerClass;
				for (int i = 0; i < 7; i++)
					_UsedCardIdx[i] = false;
			}

			public PokerHand BestHand
			{
				get { return _BestHand; }
			}

			/// <summary>
			/// Uses a recursive algorithm to evaluate all possible subsets 
			/// of the set of 7 cards that have exactly 5 elements.
			/// Don't call directly, just use Evaluate, it will handle it
			/// 
			/// To help speed things up, this doesn't return anything. 
			/// Access BestHand after calling this method for the result
			/// </summary>
			/// <param name="cardNumber">The recursion level</params>
			public void EvaluateSevenCards(Card[] hand, int cardNumber)
			{
				for (int i = cardNumber; i <= 7 - (5-cardNumber); i++)
				{
					if (!_UsedCardIdx[i])
					{
						_UsedCardIdx[i] = true;
						if (cardNumber != 4) //if not last card
						{
							EvaluateSevenCards(hand, cardNumber+1);
							_UsedCardIdx[i] = false;
						}
						else //if last card/level of recursion
						{
							var subset = new Card[5];
							int z = 0;
							
							for (int u = 0; u < 7; u++)
							{
								if (_UsedCardIdx[u])
									subset[z++] = hand[u];
							}

							PokerHand p = outer.EvaluateFiveCards(subset);
							
							if (p > _BestHand)
								_BestHand = p;

							_UsedCardIdx[i] = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Public interface to evaluation... Used to evaluate both 5 and 7 card hands
		/// </summary>
		public PokerHand Evaluate(Card[] hand)
		{
			if (hand.Length == 5)
				return EvaluateFiveCards(hand);
			if (hand.Length == 7)
			{
				var evaluator = new RecursiveSevenCardEvaluator(this);
				evaluator.EvaluateSevenCards(hand, 0);
				return evaluator.BestHand;
			}
			throw new ArgumentException("Must be either 5 or 7 cards to evaluate a hand value");
		}

		private void UpdateHistogram(Card[] hand)
		{
			//Reinitialise
			for (int i = 0; i < _CardRankHistogram.Length; i++)
				_CardRankHistogram[i] = 0;
			//Update
			foreach (Card card in hand)
			{
				if (card.IsKnown)
					_CardRankHistogram[(int)card.Rank - 1]++;
			}
		}

		private int CountPairs()
		{
			int count = 0;
			foreach (int i in _CardRankHistogram)
				if (i >= 2) count++;
			return count;
		}

		private int CountEqualRanks()
		{
			int max = 0;
			foreach (int i in _CardRankHistogram)
				if (i > max) max = i;
			return max;
		}

		private bool IsStraight()
		{
			int count = 0;
			foreach (int i in _CardRankHistogram)
			{
				if (i == 1) count++;
				else count = 0;

				if (count == 5) return true;
			}

			return count == 4 && _CardRankHistogram[0] == 1; //ace can be high or low card
		}

		private bool IsFlush(Card[] hand)
		{
			int[] suitsCount = {0, 0, 0, 0}; //Spades, Clubs, Hearts, Diamonds in that order
			foreach (Card card in hand)
			{
				switch (card.Suit) 
				{
					case Suit.Spades:   suitsCount[0]++; break;
					case Suit.Clubs:    suitsCount[1]++; break;
					case Suit.Hearts:   suitsCount[2]++; break;
					case Suit.Diamonds: suitsCount[3]++; break;
				}
			}

			foreach (int i in suitsCount)
				if (i == 5) return true;
			return false;
		}

		private Rank GetHighCard()
		{
			if (_CardRankHistogram[0] > 0) return Rank.Ace; //ace is at low end of list, but is considered high rank in poker
			for (int i = _CardRankHistogram.Length - 1; i > 0; i--)
				if (_CardRankHistogram[i] > 0) return (Rank)i + 1;
			return Rank.Two; //stops c# compiler winging
		}       
	}
}
