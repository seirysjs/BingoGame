using System;
using System.Collections.Generic;

namespace BingoGame 
{ 
    class Program
    {
        //Run Bingo Game with config: BallsCount 60; BallsDrawn 30; rowsPerTicket 3;
        static public InstanceConfig RunConfig = new InstanceConfig(60, 30, 3);
        static public BingoGame GameInstance { get; private set; }
        static void Main(string[] args)
        {
            Console.WriteLine("* B I N G O * Game!");
            GameInstance = new BingoGame(RunConfig);

            Console.ReadLine();
        }
    }

    public class BingoGame
    {
        public BallsCounter BallsInstance { get; set; }
        public InstanceConfig RoundRules { get; set; }
        public BingoGame(InstanceConfig instanceConfig)
        {
            RoundRules = instanceConfig;
            Console.WriteLine(
                "BallsCount {0}. \nBallsDrawn {1}. \nRowsPerTicket {2}. \n",
                RoundRules.BallsCount,
                RoundRules.BallsDrawn,
                RoundRules.RowsPerTicket
            );

            //init BINGO Game's balls' instance
            BallsInstance = new BallsCounter(RoundRules.BallsCount);
            BallsInstance.GameBalls.Balls.ForEach(LetterBalls => LetterBalls.Balls.ForEach(Ball => Console.WriteLine("{0}-({1})", LetterBalls.Letter, Ball)));


        }
    }

    public class InstanceConfig
    {
        public InstanceConfig(int ballsCount, int ballsDrawn, int rowsPerTicket)
        {
            BallsCount = ballsCount;
            BallsDrawn = ballsDrawn;
            RowsPerTicket = rowsPerTicket;
        }

        public int BallsCount { get; }
        public int BallsDrawn { get; }
        public int RowsPerTicket { get; }
    }

    public class BallsCounter
    {
        public BingoBalls GameBalls { get; set; } = new BingoBalls();

        public BallsCounter(int ballsCount)
        {
            // By Bingo rules you require 15 balls for B I N G O ball count differencial
            // 5xB; 4xI; 3xN; 2xG; 1xO;
            if (ballsCount < 15) 
            {
                Console.WriteLine("Bingo Game requires at least 15 Balls!");
                return;
            }  

            int baseBallsCount = ballsCount - 15; // total number of balls to use for division 
            int numbersLeft = baseBallsCount % 5 ; // leftover balls to divide each by 1 for letter
            int numbersBaseCount = (baseBallsCount - numbersLeft) / 5; // count of balls for each letter ex.: (B)

            string[] bingoLetters = new string[] { "B", "I", "N", "G", "O" }; // BINGO letter to mark/tie balls with numbers
            int lastBingoBallNumber = 0; // last ball index used in marking balls to numbers
            for (int indexBingoLetter = 0; indexBingoLetter < bingoLetters.Length; indexBingoLetter++)
            {
                // Balls for 1 letter, letter used from letters array
                LetterBalls letterBalls = new LetterBalls(bingoLetters[indexBingoLetter]);
                
                int letterBallsCount = numbersBaseCount;
                // Dividing leftover balls across B I N G O balls
                if (numbersLeft > 0)
                {
                    letterBallsCount++;
                    numbersLeft--;
                }

                // B I N G O letters differencial
                letterBallsCount += 5 - indexBingoLetter;

                letterBalls.Balls = new List<int> { };
                // Filling letter's balls list with numbers
                for (int indexletterBall = 0; indexletterBall < letterBallsCount; indexletterBall++)
                {
                    lastBingoBallNumber++;
                    int ballNumber = lastBingoBallNumber;
                    letterBalls.Balls.Add(ballNumber);
                }

                // adding formed letter ball list to BINGO balls list
                GameBalls.Balls.Add(letterBalls);
            }
        }

    }

    public class BingoBalls
    {
        public List<LetterBalls> Balls { get; set; } = new List<LetterBalls>();
        public BingoBalls()
        {
            //
        }
    }

    public class LetterBalls
    {
        public string Letter { get; set; }
        public List<int> Balls { get; set; }
        public LetterBalls(string letter)
        {
            Letter = letter;
            Balls = new List<int> { };
        }
    }
}
