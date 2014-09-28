using PokerOddsCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Poker_Evaluator_Tests
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
	
	/// <summary>
	///This is a test class for PokerHandEvaluatorTest and is intended
	///to contain all PokerHandEvaluatorTest Unit Tests
	///</summary>
	[TestClass()]
	public class PokerHandEvaluatorTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		[TestMethod()]
		public void HighCard()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Two, Suit.Spades);
			hand[2] = new Card(Rank.Four, Suit.Hearts);
			hand[3] = new Card(Rank.Seven, Suit.Diamonds);
			hand[4] = new Card(Rank.Nine, Suit.Diamonds);

			PokerHand expected = PokerHand.HighCard;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected High Card, got " + actual.ToString());
		}

		[TestMethod()]
		public void Pair()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Ace, Suit.Spades);
			hand[2] = new Card(Rank.Four, Suit.Hearts);
			hand[3] = new Card(Rank.Seven, Suit.Diamonds);
			hand[4] = new Card(Rank.Nine, Suit.Diamonds);

			PokerHand expected = PokerHand.Pair;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Pair, got " + actual.ToString());
		}

		[TestMethod()]
		public void TwoPair()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Ace, Suit.Spades);
			hand[2] = new Card(Rank.Four, Suit.Hearts);
			hand[3] = new Card(Rank.Seven, Suit.Diamonds);
			hand[4] = new Card(Rank.Seven, Suit.Clubs);

			PokerHand expected = PokerHand.TwoPair;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Two Pair, got " + actual.ToString());
		}

		[TestMethod()]
		public void ThreeOfAKind()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Ace, Suit.Spades);
			hand[2] = new Card(Rank.Ace, Suit.Clubs);
			hand[3] = new Card(Rank.Seven, Suit.Diamonds);
			hand[4] = new Card(Rank.Nine, Suit.Diamonds);

			PokerHand expected = PokerHand.ThreeOfAKind;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Three of a Kind, got " + actual.ToString());
		}

		[TestMethod()]
		public void Straight()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Two, Suit.Spades);
			hand[2] = new Card(Rank.Three, Suit.Hearts);
			hand[3] = new Card(Rank.Four, Suit.Diamonds);
			hand[4] = new Card(Rank.Five, Suit.Diamonds);

			PokerHand expected = PokerHand.Straight;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Straight, got " + actual.ToString());
		}

		[TestMethod()]
		public void Flush()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Two, Suit.Hearts);
			hand[2] = new Card(Rank.Four, Suit.Hearts);
			hand[3] = new Card(Rank.Seven, Suit.Hearts);
			hand[4] = new Card(Rank.Nine, Suit.Hearts);

			PokerHand expected = PokerHand.Flush;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Flush, got " + actual.ToString());
		}

		[TestMethod()]
		public void FullHouse()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Ace, Suit.Spades);
			hand[2] = new Card(Rank.Four, Suit.Hearts);
			hand[3] = new Card(Rank.Four, Suit.Diamonds);
			hand[4] = new Card(Rank.Ace, Suit.Diamonds);

			PokerHand expected = PokerHand.FullHouse;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Full House, got " + actual.ToString());
		}

		[TestMethod()]
		public void FourOfAKind()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Ace, Suit.Spades);
			hand[2] = new Card(Rank.Ace, Suit.Clubs);
			hand[3] = new Card(Rank.Ace, Suit.Diamonds);
			hand[4] = new Card(Rank.Nine, Suit.Diamonds);

			PokerHand expected = PokerHand.FourOfAKind;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Four of a Kind, got " + actual.ToString());
		}

		[TestMethod()]
		public void StraightFlush()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ace, Suit.Hearts);
			hand[1] = new Card(Rank.Two, Suit.Hearts);
			hand[2] = new Card(Rank.Three, Suit.Hearts);
			hand[3] = new Card(Rank.Four, Suit.Hearts);
			hand[4] = new Card(Rank.Five, Suit.Hearts);

			PokerHand expected = PokerHand.StraightFlush;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Straight Flush, got " + actual.ToString());
		}

		[TestMethod()]
		public void RoyalFlush()
		{
			Card[] hand = new Card[5];
			hand[0] = new Card(Rank.Ten, Suit.Hearts);
			hand[1] = new Card(Rank.Jack, Suit.Hearts);
			hand[2] = new Card(Rank.Queen, Suit.Hearts);
			hand[3] = new Card(Rank.King, Suit.Hearts);
			hand[4] = new Card(Rank.Ace, Suit.Hearts);

			PokerHand expected = PokerHand.RoyalFlush;
			PokerHand actual = (PokerHand)(new PokerHandEvaluator()).Evaluate(hand);
			Assert.AreEqual(expected, actual, "Expected Royal Flush, got " + actual.ToString());
		}
	}
}
