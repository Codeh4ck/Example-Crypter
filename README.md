# Example Crypter
Example crypter is a project demonstrating how executable files can be encrypted and injected into memory (without the payload being dropped on the disk) using a stub file.

# Features
* Native resources storage method
* Managed resources storage method (CodeDom)
* Simple AES encryption
* RunPE Injection (injects to vbc.exe)
* Add file to startup
* Hide file from the file explorer
* Change the stub's icon using CodeDom
* Commented in the most critical parts

# Disclaimer
This source code was written for educational purposes. Do not expect the outputs to be not detectable by antivirus solutions. Some tips might include moving the RunPE to an encrypted DLL in resources, refactoring the code, replacing methods with alternative ones etc. The source is commented on its most critical parts (compiling, encrypting, structuring the stub). The stub is also commented to help you get an idea. 

# Screenshot
![Imgur](https://i.imgur.com/GbHBaav.png)

_**Enjoy and happy learning!**_
