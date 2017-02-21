/* 
Kaylee Nevin
CSC 160-470
Project: Hangman
Date: July 29, 2015
Description: Build hangman game that prints all words in a file, selects the last word as key word, 
			and allows user to guess letters that may be in the word. Print the appropriate hangman
			score board for each guess made by user. 
*/

#include<iostream>
#include<string>
#include<fstream>
#include<cctype>
#include<cstring>
#include "randword.h"
#include "MyFuncts.h"
#include "player.h"
#include "scorekeeper.h"

using namespace std; 

const string PHASE1 = "\n  --------|\n  |       |\n          |\n          |\n          |\n      ------";
const string PHASE2 = "\n  --------|\n  |       |\n  0       |\n          |\n          |\n      ------";
const string PHASE3 = "\n  --------|\n  |       |\n  0       |\n  |       |\n          |\n      ------";
const string PHASE4 = "\n  --------|\n  |       |\n  0       |\n -|       |\n          |\n      ------";
const string PHASE5 = "\n  --------|\n  |       |\n  0       |\n -|-      |\n          |\n      ------";
const string PHASE6 = "\n  --------|\n  |       |\n  0       |\n -|-      |\n /        |\n      ------";
const string PHASE7 = "\n  --------|\n  |       |\n  0       |\n -|-      |\n / \\      |\n      ------";
const string makeAGuess = "<<<<<<<<<<<<<<<<<<<<<MAKE A GUESS>>>>>>>>>>>>>>>>>>>>>>\n\n";

bool checkGuess(string wordToGuess, char guess);
bool binarySearch(char usedLetters[], int used, char letterToFind);
bool checkForWin(string hiddenWord, string wordToGuess);
void drawHangman(int boardNum);
void playGame(string wordToGuess, Player player, Player computer, ScoreKeeper& keepScore);
void bubbleSort(char usedLetters[], int used);
string maskWord(string word);
string revealWord(string word, string maskedWord, char guess);

int main()
{
	Player player; 
	Player computer; 
	computer.setName("Computer");
	ScoreKeeper keepScore; 
	keepScore.addPlayer(player);
	keepScore.addPlayer(computer);

	int promptErr = 0;
	int play = 0;
	string promptReply = " ";
	string word = " ";

	getWords("hangman.dat");

	//Prompt to play game
	do {
		word = strToUpper(getNextWord());

		cout << "\n\n----------------HANGMAN GAME----------------" << endl; 
		keepScore.printScoreKeeperInfo(); 
		cout << "\nWould you like to play hangman? (y or n)" << endl;
		cin >> promptReply;

		play = promptYN(promptReply);

		if (word == "")
		{
			cout << "There are no words left to guess. Goodbye." << endl;
			break;
		}
		else if (play == ERROR)
			promptErr++;
		else if (play == PLAY)
		{
			cout << "Let's play hangman!" << endl;
			playGame(word, player, computer, keepScore);
			promptErr = 0;
		}
		else
			break;
		
	} while (promptErr < 3);

	system("pause");
	return(0);
}

void drawHangman(int boardNum) 
{
	//switch selects appropriate hangman score board based on the number of incorrect guesses made
	switch (boardNum)
	{
	case 0: cout << PHASE1 << endl;
		break;
	case 1: cout << PHASE2 << endl;
		break;
	case 2: cout << PHASE3 << endl;
		break;
	case 3: cout << PHASE4 << endl;
		break;
	case 4: cout << PHASE5 << endl;
		break;
	case 5: cout << PHASE6 << endl;
		break;
	case 6: cout << PHASE7 << endl;
		cout << "You lose the game! Try again!" << endl;
		break;
	default: cout << "Error." << endl;
		break;
	}
}

bool checkForWin(string hiddenWord, string wordToGuess)
{
	if (hiddenWord == wordToGuess)
		return(true);
	return(false);
}

void playGame(string wordToGuess, Player player, Player computer, ScoreKeeper& keepScore)
{
	int incorrect = 0;
	int guessCount = 0;
	char guess = ' ';
	char guessedLetters[26] = { '\0' };
	bool incorrectCount = false;
	bool alreadyGuessed = false;
	string hidden = maskWord(wordToGuess);
	string showCorrect; 

	cout << "Word to Guess: " << hidden;
	
	cout << PHASE1 << endl;

	//While loop limits user to six guesses
	while (incorrect < 6)
	{
		//User prompt for character guess, translate guess to uppercase, and print guess
		cout << makeAGuess; 
		cout << "Please enter the letter you would like to guess: " << endl;
		cin >> guess;
		guess = toupper(guess); 
		cout << "Your guess is the letter: " << guess << endl;
		
		cout << "Guessed Letters: ";
		for (int i = 0; i < guessCount; i++)
			cout << guessedLetters[i] << " ";

		//Processing guess for correctness, multiple occurance
		alreadyGuessed = binarySearch(guessedLetters, guessCount, guess);
		incorrectCount = checkGuess(wordToGuess, guess);

		if (!alreadyGuessed)
		{
			guessedLetters[guessCount] = guess;
			guessCount = strlen(guessedLetters);
			bubbleSort(guessedLetters, guessCount);
		}
		else if (alreadyGuessed && !incorrectCount)
		{
			incorrect--;
			cout << "\n\nYou have already guessed that letter, please try again." << endl;
		}
		else
			cout << "\n\nYou have already guessed that letter, please try again." << endl;

		if (!incorrectCount) 
		{
			cout << "\n" << guess << " is NOT in the word to guess." << endl;
			incorrect++;
		}
		else 
		{
			cout << "\nCorrect! You guessed one letter correctly.\n" << endl;
			hidden = revealWord(wordToGuess, hidden, guess);
		}	
		//Output - corresponding hangman board and the newly revealed chars 
		cout << hidden << endl;
		drawHangman(incorrect);
		//Checks for a win
		if (checkForWin(hidden, wordToGuess) == true)
		{
			keepScore.IncrementPlayerScore(player);
			cout << "You guessed the word! WINNER! The word was: " << wordToGuess << endl;
			break;
		}
	}
	if (checkForWin(hidden, wordToGuess) != true)
	{
		cout << "\nThe word to guess was: " << wordToGuess << endl;
		keepScore.IncrementPlayerScore(computer);
	}	
}

bool checkGuess(string wordToGuess, char guess)
{
	//for loop checks each character in word string for user guess
	for (int l = 0; l < wordToGuess.length(); l++)
	{
		if (guess == wordToGuess[l])
			return(true);
	}
	return(false);
}

bool binarySearch(char usedLetters[], int used, char letterToFind)
{
	int base = 0, middle = 0, last = used - 1;
	
	while (used > 0 && base <= last)
	{
		middle = (base + last) / 2;
		if (letterToFind == usedLetters[middle])
			return(true);
		else if (usedLetters[middle] < letterToFind)
			base = middle + 1;
		else
			last = middle - 1;	
	}
	return(false);
}

void bubbleSort(char usedLetters[], int used)
{
	char temp;  
	if (used != 0)
	{
		for (int k = 0; k < used - 1; k++)
		{
			if (usedLetters[k] > usedLetters[k + 1])
			{
				temp = usedLetters[k];
				usedLetters[k] = usedLetters[k + 1];
				usedLetters[k + 1] = temp;
			}
			for (int j = 0; j < used - 1; j++)
				if (usedLetters[j] > usedLetters[j + 1])
				{
					temp = usedLetters[j];
					usedLetters[j] = usedLetters[j + 1];
					usedLetters[j + 1] = temp;
				}
		}
	}
}

string maskWord(string word)
{
	//Takes original word and converts it into "blank spaces"
	for (int a = 0; a < word.length(); a++)
	{
		word[a] = '_';
	}
	return(word);
}

string revealWord(string wordToGuess, string maskedWord, char guess)
{
	string guessStr;  
	string update; 
	int indexS = 0;
	int index = 0;

	//Converts char guess variable into string data type
	guessStr.append(1, guess);

	//Finds first instance of guess and places it in word without revealing other chars
	index = wordToGuess.find_first_of(guessStr, 0);
	update = maskedWord.replace(index, 1, guessStr);

	//Checks for second instance of a character
	indexS = wordToGuess.find_first_of(guessStr, (index+1));
	if (indexS != -1)
		update = maskedWord.replace(indexS, 1, guessStr);
	//return updated string with newly revealed chars
	return(update);
}


