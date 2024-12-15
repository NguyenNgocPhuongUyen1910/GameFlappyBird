
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Runtime.CompilerServices;
using NAudio.Wave;

namespace GameFlappyBird
{
    class ProgramFlappyBird
    {
        public ConsoleKeyInfo keypress = new ConsoleKeyInfo(); // Variable to store the key pressed by the player
        public Random rand = new Random();

        public StreamReader sr;// Read highest score from a storage file
        public StreamWriter sw;// Save highest score to a storage file

        public int score, highscore, pivotX, pivotY, height, width, falldelay, wingDelay, raiseSpeed, fallSpeed;

        public int[,] birdX = new int[5, 5]; // Matrix to store coordinates of each part of the bird
        public int[,] birdY = new int[5, 5];
        public char[,] bird = new char[5, 5]; // Matrix to store characters representing each part of the bird  
        private char wing;

        // Variables related to the first pipe
        public int[,] pipeX = new int[30, 30];// Matrix to store coordinates of each part of the first pipe
        public int[,] pipeY = new int[30, 30];
        public char[,] pipe = new char[30, 30];// Matrix to store characters representing each part of the fist pipe 
        public int splitStart, splitLength, pipePivotX, pipeWidth, r, splitDirection;

        // Variables related to the second pipe
        public int[,] pipeX2 = new int[30, 30]; // Matrix to store coordinates of each part of the second pipe
        public int[,] pipeY2 = new int[30, 30];
        public char[,] pipe2 = new char[30, 30];// Matrix to store characters representing each part of the second pipe
        public int splitStart2, splitLength2, pipePivotX2, splitDirection2;

        // State variables
        private bool gameOver, restart, isfly, isPrinted;

        // File storage variables
        private string dirFile = "D:\\FlappyBird";
        private string highscoreFile = "D:\\FlappyBird\\saved_game.txt";

        // Variable for bird color
        private ConsoleColor birdColor = ConsoleColor.Yellow; // Default is yellow

        // Sound variables
        private IWavePlayer waveOutFly; // Sound player
        private AudioFileReader audioFileFly; // Read the sound file
        private IWavePlayer waveOutHit;
        private AudioFileReader audioFileHit;
        private IWavePlayer waveOutBackground;
        private AudioFileReader audioFileBackground;


        // Function to display the main menu of the game 
        void ShowMainMenu()
        {
            int choiceID;

            Console.Clear();// Clear the screen before displaying the menu
            Console.Title = "Flappy Bird";
            Console.ForegroundColor = ConsoleColor.Green;// Set text color to green
            Console.WriteLine("||========================================================||");
            Console.WriteLine("||--------------------------------------------------------||");
            // Display the menu interface of the game
            Console.Write("||---------------------- ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("FLAPPY BIRD");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ---------------------||");

            Console.Write("||-------------------- ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Console version");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" -------------------||");
            Console.WriteLine("||========================================================||");

            pivotX = 30; // X-axis coordinate of the bird
            pivotY = 10;// Y-axis coordinate of the bird
            SetBirdInfo('v', 'o');// Set the shape of the bird

            // Draw the bird on the screen
            for (int i = 6; i < 14; i++) // Loop through rows of the bird's image area 
            {
                for (int j = 0; j < width; j++)// Loop through colunms within the screen width
                {
                    isPrinted = false;// Set a flag to check if a character has been printed 
                    for (int m = 0; m < 3; m++)// Loop through rows in the bird matrix
                        for (int n = 0; n < 5; n++)// Loop through colunms in the bird matrix
                        {
                            if (j == birdX[m, n] && i == birdY[m, n])// if the coordinates match a coordinate of a bird's part
                            {
                                // Draw and color each part of the bird based on its positions
                                if (j == pivotX + 1 && i == pivotY)
                                {
                                    Console.ForegroundColor = ConsoleColor.White; // white for the eyes
                                    Console.Write(bird[m, n]);
                                }
                                else if (j == pivotX - 1 && i == pivotY)
                                {
                                    Console.ForegroundColor = ConsoleColor.Magenta; // magenta for the wing
                                    Console.Write(bird[m, n]);
                                }
                                else if (j == pivotX + 2 && i == pivotY)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red; // red for the beak
                                    Console.Write(bird[m, n]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(bird[m, n]);
                                }
                                isPrinted = true;
                            }
                        }
                    if (!isPrinted)// if nothing has been printed at this coordinate
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" ");// Print a space to maintain the layout
                    }
                }
                Console.WriteLine();
            }
            // Display menu options
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("                    PLAY GAME   - press 1                  ");
            Console.WriteLine("                    HIGHSCORE   - press 2                  ");
            Console.WriteLine("                    CHANGE SKIN - press 3                  ");
            Console.WriteLine("                    HELP        - press 4                  ");
            Console.WriteLine("                    CREDITS     - press 5                  ");
            Console.WriteLine("                    QUIT GAME   - press 6                  ");
            Console.WriteLine();
            Console.WriteLine();

            // Process player's menu choice
            while (true) // The loop waits for the player to press a valid key
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.D1)
                {
                    choiceID = 1;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D2)
                {
                    choiceID = 2;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D3)
                {
                    choiceID = 3;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D4)
                {
                    choiceID = 4;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D5)
                {
                    choiceID = 5;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D6)
                {
                    choiceID = 6;
                    break;
                }
            }

            // Call corresponding functions based on the choice
            switch (choiceID)
            {
                case 1:
                    SelectGameMode();
                    LoadScene();
                    break;

                case 2:
                    ViewHighScore();
                    break;

                case 3:
                    ChangeBirdSkin();
                    break;

                case 4:
                    ViewHelp();
                    break;

                case 5:
                    ViewCredits();
                    break;
                case 6:
                    AreUsure("quit");
                    break;

            }
        }

        // Function to change bird's skin color 
        void ChangeBirdSkin()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("||---------------------- FLAPPY BIRD ---------------------||");
            Console.WriteLine("                   ** CHANGE BIRD SKIN **");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                     Red     -  press 1");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                     Green   -  press 2");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("                     Blue    -  press 3");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                     Yellow  -  press 4");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("||---------------------------------------------------------||");

            // Process player's color selection
            while (true)
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.D1)
                {
                    birdColor = ConsoleColor.Red; // Change bird's skin color to red
                    break;
                }
                else if (keypress.Key == ConsoleKey.D2)
                {
                    birdColor = ConsoleColor.Green; // Change bird's skin color to green
                    break;
                }
                else if (keypress.Key == ConsoleKey.D3)
                {
                    birdColor = ConsoleColor.Blue; // Change bird's skin color to blue
                    break;
                }
                else if (keypress.Key == ConsoleKey.D4)
                {
                    birdColor = ConsoleColor.Yellow; // Change bird's skin color to red
                    break;
                }
            }

            // Notify success and return to the main menu
            Console.SetCursorPosition(15, 12);
            Console.ForegroundColor = birdColor;
            Console.WriteLine("Bird body color changed successfully!");
            Thread.Sleep(1000);
            ShowMainMenu();
        }

        // Function to display highest score to the player
        void ViewHighScore()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("||---------------------- FLAPPY BIRD ---------------------||");
            Console.WriteLine("                    ** Console verion **");
            Console.WriteLine("                        <<< HELP >>>");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("                        High score: " + highscore);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("           -- Press ESC to return to Main Menu --");
            while (true) // Loop until the player presses Esc
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.Escape)
                    break;
            }
            ShowMainMenu(); // Return to the main menu
        }

        // Function to display game credits
        void ViewCredits()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("||---------------------- FLAPPY BIRD ---------------------||");
            Console.WriteLine("                    ** Console verion **");
            Console.WriteLine("                      <<< CREDITS >>>");
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("         Written in C Sharp language on 10/6/2017");
            Console.WriteLine("                   Author: Phan Phu Hao");
            Console.WriteLine("                    fb.com/ph77894456");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("           -- Press ESC to return to Main Menu --");

            while (true)// Loop until the player presses Esc
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.Escape)
                    break;
            }
            ShowMainMenu();// Return to the main menu
        }

        // Function to display game help
        void ViewHelp()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("||---------------------- FLAPPY BIRD ---------------------||");
            Console.WriteLine("                    ** Console verion **");
            Console.WriteLine("                        <<< HELPS >>>");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("         Keep the bird flying and avoiding the pipes");
            Console.WriteLine("     Don't let him touch the ground or the top of window");
            Console.WriteLine();
            Console.WriteLine("    Keyboard buttons: - Press Spacebar to raise the bird");
            Console.WriteLine("                      - Press R to restart game");
            Console.WriteLine("                      - Press ESC to pause game");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("           -- Press ESC to return to Main Menu --");
            while (true) // Loop until the player presses Esc
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.Escape)
                    break;
            }
            ShowMainMenu(); // Return to the main menu
        }

        void AreUsure(string Case)
        {
            while (true) // Loop until the player makes a decision
            {
                if (Case == "quit") // For "quit" case, display options to confirm quitting
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.CursorTop = 15; // Set the cursor at the line 15
                    Console.WriteLine();
                    Console.WriteLine(" |--------------------------------------------------------|");
                    Console.WriteLine(" |                DO YOU WANT TO QUIT GAME?               |");
                    Console.WriteLine(" |                      Yes - Press 1                     |");
                    Console.WriteLine(" |                      No  - Press 2                     |");
                    Console.Write(" |--------------------------------------------------------|");
                    // Exit the game or return to the main menu based on the choice
                    keypress = Console.ReadKey(true);
                    if (keypress.Key == ConsoleKey.D1)
                        Environment.Exit(0);
                    if (keypress.Key == ConsoleKey.D2)
                        ShowMainMenu();
                }
                /*else if (Case == "return")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.CursorTop = 10;
                    Console.WriteLine();
                    Console.WriteLine("        |------------------------------------------|        ");
                    Console.WriteLine("        |          DO YOU WANT TO STOP GAME        |        ");
                    Console.WriteLine("        |        AND RETURN TO THE MAIN MENU?      |        ");
                    Console.WriteLine("        |               Yes - Press 1              |        ");
                    Console.WriteLine("        |               No  - Press 2              |        ");
                    Console.Write    ("        |------------------------------------------|        ");

                    keypress = Console.ReadKey(true);
                    if (keypress.Key == ConsoleKey.D1)
                        ShowMainMenu();
                    if (keypress.Key == ConsoleKey.D2)
                    {
                        break;
                    }
                }*/
            }
        }

        // Function to countdown from 3 to 1 
        void CountDown()
        {
            // Console.CursorTop = 20;
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 3; i >= 1; i--)
            {
                Console.SetCursorPosition(width / 2 - 8, height / 2 + 5);// Set the cursor in the middle of the screen
                Console.Write("Ready to play: " + i);
                Thread.Sleep(1000);// Pause for 1 second
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }

        // Function to pause the game
        void Pause()
        {
            Console.CursorTop = 10;// Set the cursor at the line 10
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("        ============================================        ");
            Console.WriteLine("                         GAME PAUSED                        ");
            Console.WriteLine("              Resume game             - press 1             ");
            Console.WriteLine("              Restart game            - press 2             ");
            Console.WriteLine("              Return to the Main Menu - press 3             ");
            Console.WriteLine("        ============================================        ");
            Console.ForegroundColor = ConsoleColor.Green;

            while (true) // Loop until player presses a valid key
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.D1)
                {
                    Render();
                    goto through1;
                }
                else if (keypress.Key == ConsoleKey.D2)
                {
                    restart = true;
                    goto through2;
                }
                else if (keypress.Key == ConsoleKey.D3)
                    break;
            }
            ShowMainMenu();
        through1:;
            CountDown();
        through2:;
        }

        void Lose()
        {
            waveOutBackground.Stop(); // Stop background music
            Console.CursorTop = 10;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("        ============================================        ");
            Console.WriteLine("                          GAME OVER                         ");
            Console.WriteLine("                        Your score: {0}                     ", score);
            Console.WriteLine("                        High score: {0}                     ", highscore);
            Console.WriteLine("               Return to Main Menu    - press 1               ");
            Console.WriteLine("               Restart game           - press 2               ");
            Console.WriteLine("               Select a new game mode - press 3               ");
            Console.Write("        ============================================        ");
            Console.ForegroundColor = ConsoleColor.Green;

            while (true)// Loop until the player presses a valid key
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.D1)
                {
                    goto through1;
                }
                if (keypress.Key == ConsoleKey.D2)
                {
                    goto through2;
                }
                if (keypress.Key == ConsoleKey.D3)
                {
                    goto through3;
                }
            }
        through1:;
            ShowMainMenu(); // Return to the main menu
            return;
        through2:;
            restart = true; // Set the state restart
            return;
        through3:;
            SelectGameMode(); // Call the function to select game mode
            restart = true; // Restart with the selected game mode
            return;
        }

        // Function to configure the bird 
        void SetBirdInfo(char wch, char ech)
        {
            bird[0, 0] = ' '; birdX[0, 0] = pivotX - 2; birdY[0, 0] = pivotY - 1;
            bird[0, 1] = '='; birdX[0, 1] = pivotX - 1; birdY[0, 1] = pivotY - 1;
            bird[0, 2] = '='; birdX[0, 2] = pivotX; birdY[0, 2] = pivotY - 1;
            bird[0, 3] = '='; birdX[0, 3] = pivotX + 1; birdY[0, 3] = pivotY - 1;
            bird[0, 4] = ' '; birdX[0, 4] = pivotX + 2; birdY[0, 4] = pivotY - 1;

            bird[1, 0] = '='; birdX[1, 0] = pivotX - 2; birdY[1, 0] = pivotY;
            bird[1, 1] = wch; birdX[1, 1] = pivotX - 1; birdY[1, 1] = pivotY; // wing
            bird[1, 2] = '='; birdX[1, 2] = pivotX; birdY[1, 2] = pivotY; // pivot
            bird[1, 3] = ech; birdX[1, 3] = pivotX + 1; birdY[1, 3] = pivotY; // eye
            bird[1, 4] = '>'; birdX[1, 4] = pivotX + 2; birdY[1, 4] = pivotY; // neb

            bird[2, 0] = ' '; birdX[2, 0] = pivotX - 2; birdY[2, 0] = pivotY + 1;
            bird[2, 1] = '='; birdX[2, 1] = pivotX - 1; birdY[2, 1] = pivotY + 1;
            bird[2, 2] = '='; birdX[2, 2] = pivotX; birdY[2, 2] = pivotY + 1;
            bird[2, 3] = '='; birdX[2, 3] = pivotX + 1; birdY[2, 3] = pivotY + 1;
            bird[2, 4] = ' '; birdX[2, 4] = pivotX + 2; birdY[2, 4] = pivotY + 1;
        }

        // Function to configure the first pipe
        void SetPipeInfo()
        {
            // Set coordinates for each part of the first pipe
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < pipeWidth; j++)
                {
                    if (j < r)
                        pipeX[i, j] = pipePivotX - (r - j);
                    else if (j > r)
                        pipeX[i, j] = pipePivotX + (j - r);
                    else if (j == r)
                        pipeX[i, j] = pipePivotX;

                    pipeY[i, j] = i;
                    pipe[i, j] = '='; // Set characters representing for each part of the first pipe.
                }
            }

            // Create empty space of the pipe 
            for (int k = splitStart; k < splitLength + splitStart; k++)
            {
                // Replace the items in the empty space with spaces
                for (int l = 0; l < pipeWidth; l++)
                {
                    pipe[k, l] = ' ';
                }
            }
        }

        // Function to configure the second pipe
        void SetPipe2Info()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < pipeWidth; j++)
                {
                    if (j < r)
                        pipeX2[i, j] = pipePivotX2 - (r - j);
                    else if (j > r)
                        pipeX2[i, j] = pipePivotX2 + (j - r);
                    else if (j == r)
                        pipeX2[i, j] = pipePivotX2;

                    pipeY2[i, j] = i;
                    pipe2[i, j] = '=';
                }
            }

            for (int k = splitStart2; k < splitLength2 + splitStart2; k++)
            {
                for (int l = 0; l < pipeWidth; l++)
                {
                    pipe2[k, l] = ' ';
                }
            }
        }

        // Function to check if the bird hits the screen boundaries or pipes
        void DeadCheck()
        {
            // Check if the bird goes out of screen boundaries
            if (pivotY + 1 <= 2 || pivotY + 1 >= height - 1) // If the bird touches the top or bottom boundaries
            {
                SetBirdInfo(wing, 'x'); // Change the bird's shape to represent collision
                Render(); // Redraw the screen
                waveOutHit.Play(); // Play collision sound 
                gameOver = true; // Set the game over state
            }

            // Check if the bird collides with pipes
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {

                    if (birdY[i, j] <= pipeY[splitStart, 0] - 1 || birdY[i, j] >= pipeY[splitStart + splitLength, 0])
                    {
                        if (birdX[i, j] >= pipePivotX - r && birdX[i, j] <= pipePivotX + r - 1)
                        {
                            SetBirdInfo(wing, 'x');
                            Render();
                            waveOutHit.Play();
                            gameOver = true;
                        }
                    }

                    if (birdY[i, j] <= pipeY2[splitStart2, 0] - 1 || birdY[i, j] >= pipeY2[splitStart2 + splitLength2, 0])
                    {
                        if (birdX[i, j] >= pipePivotX2 - r && birdX[i, j] <= pipePivotX2 + r + 1)
                        {
                            SetBirdInfo(wing, 'x');
                            Render();
                            waveOutHit.Play();
                            gameOver = true;
                        }
                    }
                }
            }
        }

        // Function to read the highest score from a file
        void ReadHighScoreFromFile()
        {
            try
            {   // Try to open and read from file
                string num;
                sr = new StreamReader(highscoreFile);
                while ((num = sr.ReadLine()) != null)
                {
                    highscore = int.Parse(num);
                }
                sr.Close(); // Close the file after reading
            }
            catch
            {   // If the file doesn't exist, create it and set default highest score to 0
                Directory.CreateDirectory(dirFile);
                File.Create(highscoreFile);
                highscore = 0;
                //WriteHighScore();
            }
        }

        // Function to set up the game parameters
        void Setup()
        {
            // Size of console screen
            height = 30;
            width = 60;

            Console.SetWindowSize(width, height + 5); // Screen size
            Console.ForegroundColor = ConsoleColor.Green; // Hide the cursor
            Console.CursorVisible = false; // Hide the cursor

            gameOver = false;
            restart = false;
            isfly = false;

            score = 0;
            falldelay = 0;
            wing = 'v';

            // Coordinate of the bird
            pivotX = 20;
            pivotY = height / 2;

            splitStart = rand.Next(5, height - 10);
            splitStart2 = rand.Next(3, height - 13);

            pipePivotX = 60;                                        
            pipePivotX2 = pipePivotX + pipeWidth + 21;
            pipeWidth = 17;
            r = pipeWidth / 2;

            // Load the sounds during initialiation. When the game starts, the sound will be ready to play
            LoadSounds();
        }

        // Function to process player's input 
        void GameCheckInput()
        {
            while (Console.KeyAvailable) // Check if there are key pressed by the player
            {
                if (!gameOver)// If the game has not ended
                    keypress = Console.ReadKey(true);// Read the key press
                if (keypress.Key == ConsoleKey.Spacebar)// If the player presses `Space`
                {
                    isfly = true;// Set the bird's flying state to true
                    waveOutFly.Play(); // Play the flying sound
                    // Replay the flying sound for looping
                    waveOutFly.Stop();
                    audioFileFly.Position = 0; // Reset the playback position to the start
                    waveOutFly.Play();
                }
                if (keypress.Key == ConsoleKey.Escape)// If the player presses `Esc`
                    Pause(); // Call the method to pause the game
            }
        }


        void Logic()
        {
            // Move the pipes to the left
            pipePivotX--;
            pipePivotX2--;
            falldelay++;
            wingDelay++;

            if (wingDelay == 1) // If the bird's wing need to change state
                wing = '^'; // The bird's wing character change to `^`

            if (falldelay == 1)// If the bird fall
            {
                pivotY += fallSpeed;// The bird's Y-axis coordinate increases (bird falls down)
                falldelay = 0;
            }
            if (isfly)// If the flying state is activated
            {
                //Console.Beep();
                pivotY -= raiseSpeed; //  The bird's Y-axis coordinate decreases (bird flies up)
                wing = 'v';// The bird's wimg returns to normal state
                falldelay = -1;
                wingDelay = -1;
                isfly = false;// Reset the flying state
            }

            if (pipePivotX == pivotX - r || pipePivotX2 == pivotX - r)// If the bird passes a pipe
            {
                score++;// Increase the score
                if (score > highscore)// If the current score is higher than the highest score
                {
                    highscore = score; // Update the highest score
                }
            }

            if (pipePivotX == -r)// If the pipe moves off the screen
            {
                pipePivotX = width + r; // Reset the pipe's position to the right of the screen
                splitStart = rand.Next(3, height - splitLength - 3);// Create a new empty space for the pipe
            }

            if (pipePivotX2 == -r)
            {
                pipePivotX2 = width + r;
                splitStart2 = rand.Next(3, height - 13);
            }

            // Make the pipe move up and down when score >=5
            if (score >= 5)
            {
                splitStart += splitDirection; // Move up or down
                splitStart2 += splitDirection2;

                // Reverse direction when reaching the limit
                if (splitStart <= 3 || splitStart + splitLength >= height - 3)
                {
                    splitDirection *= -1; // Riverse direction 
                }
                if (splitStart2 <= 3 || splitStart2 + splitLength2 >= height - 3)
                {
                    splitDirection2 *= -1;
                }
            }

            // Reset pipes and bird states
            SetPipeInfo();
            SetPipe2Info();
            SetBirdInfo(wing, 'o');
        }


        void Render()
        {
            if (!gameOver) // If the game hasn't ended
            {
                Console.SetCursorPosition(0, 0);// Set the cursor to the top-left of the screen
                Console.ForegroundColor = ConsoleColor.Green;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        isPrinted = false;
                        // Print parts of the bird
                        for (int m = 0; m < 3; m++)
                        {
                            for (int n = 0; n < 5; n++)
                            {
                                if (j == birdX[m, n] && i == birdY[m, n])
                                {
                                    if (j == pivotX + 1 && i == pivotY)
                                    {
                                        if (bird[m, n] == 'o')
                                            Console.ForegroundColor = ConsoleColor.White;
                                        else
                                            Console.ForegroundColor = ConsoleColor.Red;
                                    }
                                    else if (j == pivotX - 1 && i == pivotY)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Magenta;// Wing color
                                    }
                                    else if (j == pivotX + 2 && i == pivotY)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;// Beak color
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = birdColor;// Change the bird skin color based on selection 
                                    }

                                    Console.Write(bird[m, n]);
                                    isPrinted = true;
                                }
                            }
                        }
                        if (!isPrinted)
                        {
                            // Print pipe 1
                            for (int a = 0; a < height; a++)
                            {
                                for (int b = 0; b < pipeWidth; b++)
                                {
                                    if (j == pipeX[a, b] && i == pipeY[a, b])
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write(pipe[a, b]);
                                        isPrinted = true;
                                    }
                                }
                            }
                        }
                        if (!isPrinted)
                        {
                            // Print pipe 2
                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < pipeWidth; x++)
                                {
                                    if (j == pipeX2[y, x] && i == pipeY2[y, x])
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write(pipe2[y, x]);
                                        isPrinted = true;
                                    }
                                }
                            }
                        }

                        // Print space if nothing else to print 
                        if (!isPrinted)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(' ');
                        }
                    }
                    Console.WriteLine(); // Move to next line after completing a row
                }

                Console.SetCursorPosition(0, height); // Set the cursor at the bottom of the screen
                // Display some information 
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-----------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your score: " + score);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("FLAPPY BIRD console version");
                Console.WriteLine("Created by Phan Phu Hao");
            }
        }

        // Function to save the highest score to a file
        void WriteHighScore()
        {
            sw = new StreamWriter(highscoreFile);
            sw.WriteLine(highscore);
            sw.Close();
        }


        void Update()
        {
            Console.Clear();
            while (true)// The main game loop
            {
                GameCheckInput(); // Check input
                Logic(); // Process the game logic
                Render();// Redraw the game screen 
                DeadCheck();// Check for game over
                if (gameOver || restart)// If the game has ended or the player has chosen to restart the game 
                    break;
                //Thread.Sleep(10);
            }

            if (gameOver) // If the game has ended
            {
                try
                {
                    WriteHighScore();
                }
                catch
                { // If there is an error writing to the file, the program will skip it and continue
                    Thread.Sleep(500);// Pause 0.5s before displaying the gameover screen 
                }
                Lose();// Call the method `Lose` to display the gameover screen 
            }
        }

        // Function to initialize and start the game 
        void LoadScene()
        {
            // Display countdown, set up the game and start the main game loop
            CountDown();
            Setup();
            Update();
        }

        // Function to select the game mode
        void SelectGameMode()
        {
            // Display game mode options and process player's selection
            int choiceMode;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("||========================================================||");
            Console.WriteLine("||--------------------------------------------------------||");
            Console.Write("||---------------------- ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("FLAPPY BIRD");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ---------------------||");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("||-------------------- SELECT GAME MODE ------------------|| ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("                        Hard   - press 1                    ");
            Console.WriteLine("                        Medium - press 2                  ");
            Console.WriteLine("                        Easy   - press 3                    ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("||--------------------------------------------------------||");

            while (true)
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.D1)
                {
                    choiceMode = 1;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D2)
                {
                    choiceMode = 2;
                    break;
                }
                else if (keypress.Key == ConsoleKey.D3)
                {
                    choiceMode = 3;
                    break;
                }
            }

            // Adjust game parameters based on selected mode
            switch (choiceMode)
            {

                case 1:
                    {
                        fallSpeed = 2;
                        raiseSpeed = 4;
                        splitLength = splitLength2 = 7;
                        splitDirection = splitDirection2 = 1;
                        break;
                    }
                case 2:
                    {
                        fallSpeed = 2;
                        raiseSpeed = 4;
                        splitLength = splitLength2 = 8;
                        splitDirection = splitDirection2 = 0;
                        break;
                    }
                case 3:
                    {
                        fallSpeed = 1;
                        raiseSpeed = 3;
                        splitLength = splitLength2 = 10;
                        splitDirection = splitDirection2 = 0;
                        break;
                    }
            }
        }

        // Function to play sound 
        void LoadSounds()
        {
            try
            {
                // Flying sound
                waveOutFly = new WaveOutEvent(); //Create an audio player for the flying sound
                audioFileFly = new AudioFileReader("fly.wav"); //Load the fly.wav file 
                waveOutFly.Init(audioFileFly); //Link the auio file to the flayer

                // Hitting sound
                waveOutHit = new WaveOutEvent(); //Create an audio player for the hitting sound
                audioFileHit = new AudioFileReader("hit.wav"); //Load the hit.wav file
                waveOutHit.Init(audioFileHit); //Link the auio file to the flayer

                // Background music
                waveOutBackground = new WaveOutEvent(); //Create an audio player for the background music
                audioFileBackground = new AudioFileReader("background.wav"); //Load the background.wav file
                waveOutBackground.Init(audioFileBackground); //Link the auio file to the flayer

                waveOutBackground.Play(); // Play background music
                waveOutBackground.PlaybackStopped += (s, e) => // When background music stop
                {
                    audioFileBackground.Position = 0; // Reset the audio file position to the start
                    waveOutBackground.Play();  // Replay background music
                };
            }

            // If an audio file is missing or fails to load
            catch (Exception ex)
            {
                Console.WriteLine("Error loading audio file: " + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            ProgramFlappyBird Fb = new ProgramFlappyBird();// Create a new game object
            Fb.ReadHighScoreFromFile();// Read highest score from file
            Fb.Setup(); // Set up the game
            Fb.ShowMainMenu(); // Display main menu and start the game 
            while (true)// Loop to allow player to play multiple times
            {
                Fb.LoadScene(); // Enter the main game loop
            }
        }
    }
}