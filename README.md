# Rx64
**An emulator for the Intel x64 processor written in C#**

*NOTE: This is an old project of mine that is no longer maintained. I recently uploaded it to
GitHub for the purpose of showing it to people who may be interested.*


This project was a labor-of-love experiment of mine that I started in 2012 and only worked on at night
over the course of a couple weeks. Over the previous years (and still to this day) I'd had a fascination
with operating systems development, so this time I thought I'd take a crack at writing low-level software from 
a different angle, so to speak, by creating a program that's able to read and interpret x64 machine code.
All the while utilizing the nice OOP features that C# has to offer. Unfortunately, it never reached 64-bit
capabilities. It's currently a basic 16-bit x86 emulator. It has enough coverage of the x86 instruction set
to execute machine code containing decision-making and branching constructs, as well as data storage and
retrieval operations.


## How to Run

*NOTE: This project must be run without debugging (CTRL+F5) due to the Visual Studio debugger complaining
about non-GUI threads accessing the form controls.*

- Load the solution in Visual Studio (originally written with VS 2010, but I've tested in VS 2015)
- Build the project, in "release" mode
- Copy `bootsect.bin` into the `Rx64\bin\release` directory
- Execute `Rx64\bin\release\Rx64.exe`


## Build a Program for Rx64

If you're feeling especially adventurous, you can write x86 assembly code to target this platform.
You can find the list of implemented instructions by examining the `Rx64\VirtualMachine\CPU\Instructions`
directory. Keep in mind that some code files may not have code yet... So to be extra-sure that an
instruction is implemented, open the corresponding code file. Once you've written your program,
it must be assembled as a flat binary file. Similar to a PC, your `bootsect.bin` will be loaded
into memory at address `0x7c00`. Also similar to a PC, text-mode video memory starts at memory
address `0xb8000` (`0xb800:0x0000` in 16-bit segmented notation).
