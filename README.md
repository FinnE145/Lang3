# Lang3
A lightweight statically typed OOP scripting language with flexible structures.

> For a thorough introduction to Lang3, and more information on syntax, structures, and other aspects and features of the language, check out the [Documentation Jupyter Notebook](https://github.com/FinnE145/Lang3/blob/main/Docs/Lang3Docs.ipynb). Soon, a website will be linked here with documentation, download options, and a page to write Lang3 code in your browser and run it on our servers.

> Note that Lang3 is a placeholder name, and will be changed at a later date. If you have a name suggestion, submit an [issue](https://github.com/FinnE145/Lang3/issues/new) with the [`enhancement`](https://github.com/FinnE145/Lang3/labels/enhancement) tag.

## Download

Lang3 is still in the early development stage, and is not available for download yet. You can run Lang3 in your browser at [lang3.fmje.dev/run](https://lang3.fmje.dev/run) to see lexer and parser output of operators with numbers and variables. Also feel free to download the repo and compile the C# yourself!

## CLI syntax

`Lang3 [filepath] [args]`

*Possible args include:*

`!t`/`!tokens`: Show Tokens (lexer output)\
`!a`/`!ast`: Show AST (parser output)\
`!d`/`!debug`: Show both lexer and parser output (same as using `!t` and `!a`)\
`!v`/`!verbose`: Show more information in lexer and parser output. (Shows filepath and location in lexer output and tokens in parser output)

## What is Lang3?

Lang3 is a lightweight non-traditional OO language with static typing featuring comprehensive type prediction.

It is written in C#, will be an interpreted language (so far at least).

Objects are created flexibly with simple syntax that allows for many helpful features, including objects with a root value in addition to their properties.

Types are simple but allow for great customization, without the need for complicated structures like formal classes, interfaces, structs, etc. Instead, modify the properties and behaviors of any object (start with a generic obj for a new class, or start with something else to inherit its properties). Then, use the type of your modified object in the rest of your code, as if you had created a class or a variety of other structures. The Lang3 type and object system allows for all OOP concepts like inheritance, encapsulation, abstraction, etc.

Functions have been given a revamp. Instead of rigid definitions, assign parameters (including special default arguments separate from the function itself) to a code block object. Call the code block directly, pass it to another function (with your desired arguments included!) or assign it to a variable/object or property for future use.

Loops, conditionals, and whatever structures you write yourself, utilize the flexible object system to save you time and effort. Use the powerful combination of value and properties to save yourself from writing the same simple code again and again, and get access to a variety of helper functions and utils built into the language and structures (or easily add your own). Additionally, a powerful and easy-to-use expansion and condensation operator is built into many parts of the language.

Lang3 also has a variety of small quality-of-life improvements to existing mainstream scripting languages, originating in both the core features of the language, the design and implementation of built-ins, and syntactic sugar.

## How to use the Kernel

This repository includes an ipykernel kernel for use with Jupyter Notebooks.

> Note: Currently, all the kernel does is echo whatever is sent to it, using code from [Jupyter](https://github.com/jupyter/echo_kernel). This will be changed as Lang3 is developed. In the meantime, feel free to compile the Lang3 source code and modify the kernel to run the executable yourself, or have it run `dotnet publish` automatically on startup. It shouldn't be too difficult to implement (I just don't have the time) and you can check out a similar python implementation for the [Lang3 Website](https://lang3.fmje.dev/run) [here](https://github.com/FinnE145/Lang3Website/blob/main/codeRunner.py) to guide you. If you do it, be sure to submit a pull request!

To use the kernel, you must create a Python virtual environment and install ipykernel before installing the kernel.

**This assumes you have Python installed and are using PowerShell (or cmd)**. If this is not the case, you will have to look up how to install Python and install an IPython kernel for your OS.

1. Download this repository, unzip it if necessary, and save it in a convenient location.
2. Run `python -m venv path/to/venv` in the terminal to create a virtual environment in a given directory.
3. Run `path/to/venv/Scripts/activate`, or run one of the activate scripts in `path/to/venv/Scripts`, depending on your current terminal requirements.
4. Run `pip install ipykernel`
5. Run `pip install -e path/to/repository/Kernel` to install the kernel as a python module that will update when edited
6. Run `python -m ipykernel install --name 'EchoLang3' --prefix 'path/to/venv'`.
7. Navigate to `path/to/venv/share/jupyter/kernels/echolang3` and modify the string `"ipykernel_lancher"` in `argv` to say `"echo_kernel"`. Change the `language` to `"lang3"`.
8. Open the docs Jupyter Notebook, or any other Jupyter Notebook that you want, and select `EchoLang3` from your venv. This may vary by which program you use to open the notebook.

Now when you run cells, it should output the exact contents of the cell!

You can edit echo_kernel as you'd like and pip will automatically update the module used in the kernel. Once Lang3 works to a sufficient degree that creating a functional kernel is worth it, one will be added to the release, and many of these installation steps will be completed by the installer.
