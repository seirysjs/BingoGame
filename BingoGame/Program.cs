using System;
using System.Collections.Generic;

namespace BingoGame 
{ 
    class Program
    {
        //Run Bingo Game with config: BallsCount 60; BallsDrawn 30; rowsPerTicket 3;
        static public InstanceConfig RunConfig = new InstanceConfig(60, 30, 3);
        static public BingoGame GameInstance { get; private set; } = new BingoGame(RunConfig);
        static void Main(string[] args)
        {
            Console.WriteLine("\n* B I N G O * Game!");

            GameTicket bingoTicket = new GameTicket(GameInstance.BallsInstance.GameBalls, GameInstance.RoundRules.RowsPerTicket);            
            Console.WriteLine("\n Bingo Ticket ({0} Rows)", bingoTicket.TicketRows.Count);
            Console.Write("[#]  B   I   N   G   O");
            for (int indexTicketRow = 0; indexTicketRow < bingoTicket.TicketRows.Count; indexTicketRow++)
            {
                int[] ticketRow = bingoTicket.TicketRows[indexTicketRow];
                Console.Write("\n[{0}] ", (indexTicketRow + 1));
                if (ticketRow[0] < 10) Console.Write(" ");

                foreach (int ballNumber in ticketRow) Console.Write(ballNumber + "  ");
            }

            Console.WriteLine("\n");
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
                "\n BallsCount: {0}. \n BallsDrawn: {1}. \n RowsPerTicket: {2}.",
                RoundRules.BallsCount,
                RoundRules.BallsDrawn,
                RoundRules.RowsPerTicket
            );

            //init BINGO Game's balls' instance
            BallsInstance = new BallsCounter(RoundRules.BallsCount);
            // BallsInstance.GameBalls.Balls.ForEach(LetterBalls => LetterBalls.Balls.ForEach(Ball => Console.WriteLine("{0}-({1})", LetterBalls.Letter, Ball)));


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
                Console.WriteLine("\nBingo Game requires at least 15 Balls!");
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


    public class GameTicket
    {
        public BingoBalls TicketNumbers { get; set; }
        public List<int[]> TicketRows { get; set; }
        public GameTicket(BingoBalls gameBalls, int rowsPerTicket)
        {
            TicketNumbers = new BingoBalls();
            TicketRows = new List<int[]>();
            Random rnd = new Random();

            // Pick ball numbers for each Column (by B-I-N-G-O)
            foreach (LetterBalls letterBalls in gameBalls.Balls)
            {
                string letter = letterBalls.Letter;
                List<int> letterNumbers = letterBalls.Balls; 
                List<int> selectedNumbers = new List<int> { };

                // Pick ball numbers for each Row for the letter, ex.: (B)
                for (int indexTicketRow = 0; indexTicketRow < rowsPerTicket; indexTicketRow++)
                {
                    int numbersLeftCount = letterNumbers.Count;
                    int indexLetterNumber = rnd.Next(numbersLeftCount);
                    int letterNumber = letterNumbers[indexLetterNumber];
                    selectedNumbers.Add(letterNumber);
                    letterNumbers.RemoveAt(indexLetterNumber);
                }
                // Sort picked numbers for the letter (ASC)
                selectedNumbers.Sort();

                LetterBalls ticketColumnNumbers = new LetterBalls(letter);
                ticketColumnNumbers.Balls = selectedNumbers;

                // Add ball numbers for the column to the ticket
                TicketNumbers.Balls.Add(ticketColumnNumbers);
            }

            // Transform Ticket's B-I-N-G-O Column numbers to Rows for the game 
            string[] bingoLetters = new string[] { "B", "I", "N", "G", "O" };
            for (int indexTicketRow = 0; indexTicketRow < rowsPerTicket; indexTicketRow++)
            {
                List<int> ticketRow = new List<int> { }; 
                foreach (string ticketColumn in bingoLetters)
                {
                    LetterBalls? columnNumbers = TicketNumbers.Balls.Find(letterNumbers => letterNumbers.Letter == ticketColumn);
                    if (columnNumbers is null) return;

                    int ballNumber = columnNumbers.Balls[indexTicketRow];
                    ticketRow.Add(ballNumber);
                }

                // Add formed B-I-N-G-O Numbers Row to Ticket Row List
                TicketRows.Add(ticketRow.ToArray());
            }
        }
    }
}
