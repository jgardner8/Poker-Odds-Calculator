using System;
using System.IO;
using System.Collections.Generic;

namespace PokerOddsCalculator
{
	class Program
	{
		static void Main(string[] args)
		{
			const int DEC_PLACES = 2;
			const int SIMS_TO_RUN = 100000;
			Console.WriteLine("Running {0} simulations...\n", SIMS_TO_RUN);

			var timeList = new List<long>();
			var rand = new Random();

			while (!Console.KeyAvailable)
			{
				//Create a random hand
				var hand = new List<Card>();
				var numCards = rand.Next(6) + 1; //1-6 cards
				for (int i = 0; i < numCards; i++)
					hand.Add(new Card((Rank)rand.Next(13)+1, (Suit)rand.Next(4)));

				//Time odds calculation
				OddsCalculator calculator = new OddsCalculator();
				var timeStart = DateTime.Now.Ticks;
					float[] odds = calculator.RunSimulations(hand, SIMS_TO_RUN);
				var timeEnd = DateTime.Now.Ticks;
				timeList.Add(TimeSpan.FromTicks(timeEnd-timeStart).Milliseconds);

				//Echo results (testing)
				Console.WriteLine(numCards + " cards:");
				foreach (Card c in hand)
					if (c.IsKnown)
						Console.WriteLine("\t" + c.ToString());

				Console.WriteLine("--------");

				Console.WriteLine("High Card: \t\t" +     Math.Round(odds[0], DEC_PLACES) + "%\n"
							    + "Pair: \t\t\t" +        Math.Round(odds[1], DEC_PLACES) + "%\n"
							    + "Two Pair: \t\t" +      Math.Round(odds[2], DEC_PLACES) + "%\n"
							    + "Three of a Kind: \t" + Math.Round(odds[3], DEC_PLACES) + "%\n"
							    + "Straight: \t\t" +      Math.Round(odds[4], DEC_PLACES) + "%\n"
							    + "Flush: \t\t\t" +       Math.Round(odds[5], DEC_PLACES) + "%\n"
							    + "Full House: \t\t" +    Math.Round(odds[6], DEC_PLACES) + "%\n"
							    + "Four of a Kind: \t" +  Math.Round(odds[7], DEC_PLACES) + "%\n"
							    + "Straight Flush: \t" +  Math.Round(odds[8], DEC_PLACES) + "%\n"
							    + "Royal Flush: \t\t" +   Math.Round(odds[9], DEC_PLACES) + "%\n\n"
							    + "Simulations Ran: \t" + calculator.SimulationsRan);
				Console.WriteLine("Time Taken: \t\t" + timeList[timeList.Count-1] + "ms");
				Console.WriteLine("\n--------------------------------------\n");
			}

			using (var log = new StreamWriter(File.Create(DateTime.Now.ToString("dd-MM-yy H-mm-ss") + ".txt")))
				foreach (int i in timeList)
					log.WriteLine(i);
		}
	}
}
