Octo-concurrency
================

Crowd movement simulation programm

Authors : Ancelin ARNAUD, Gwenn AUBERT, Bastien MAUREILLE, Giacomo ROMBAUT

Folder content : 

	Here is how the project folder you just unzipped is laid out.

		Sources ==> The project code for the sequencial and concurent versions, the files are ".cs" (C#) files

		Run-Concurent ==> The binaries needed to run the concurent version of the simulation

		Run-Sequencial ==> The binaries needed to run the sequential version of the simulation

		FSP.lts ==> Our attempt to prove our scheme to manage concurent ressources

Required libraries : 

	To run the project you will need the Mono library (available on windows, linux and mac here http://www.mono-project.com/Main_Page)

Running the simulations :
	
	Once you have mono installed, simply go to Run-Concurent or Run-Sequencial folder and do one of those things :
		- Run the following command from a terminal : ./Oct-Sequential.exe or ./Octo-Concurrency.exe
		- Or you can manually right click on one of those executable files and select "run with mono"