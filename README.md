# Lang3
A lightweight statically typed OOP scripting language with flexible structures.

> For a thorough introduction to Lang3, and more information on syntax, structures, and other aspects and features of the language, check out the [Documentation Jupyter Notebook](https://github.com/FinnE145/Lang3/blob/main/Docs/Lang3Docs.ipynb). Soon, a website will be linked here with documentation, download options, and a page to write Lang3 code in your browser and run it on our servers.

> Note that Lang3 is a placeholder name, and will be changed at a later date. If you have a name suggestion, submit an [issue](https://github.com/FinnE145/Lang3/issues/new) with the [`enhancement`](https://github.com/FinnE145/Lang3/labels/enhancement) tag.

## Download

Lang3 is still in the early development stage, and is not available for download yet. Soon, a website will be added here where Lang3 can be run. In the meantime, feel free to download the repo and compile the C# yourself!

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

> Note: Currently, all the kernel does is echo whatever is sent to it, using code from [Jupyter](https://github.com/jupyter/echo_kernel). This will be changed as Lang3 is developed.

To use the kernel, you must create a Python virtual environment and install ipykernel before installing the kernel.

**This assumes you have Python installed and are using PowerShell (or cmd)**. If this is not the case, you will have to look up how to install Python and install an IPython kernel for your OS.

1. Download this repository, unzip it if necessary, and save it in a convenient location.
2. Run `python -m venv ./venv_folder_name` in the terminal to create a virtual environment in the current directory.
3. Run `./venv_folder_name/Scripts/activate`, or run one of the activate scripts in `./venv_folder_name/Scripts`, depending on your current terminal requirements.
4. Run `pip install ipykernel`
5. Run `pip install -e path/to/repository/Kernel` to install the kernel as a python module that will update when edited
6. Run `python -m ipykernel install --name 'EchoLang3' --prefix './venv_folder_name'`.
7. Navigate to `./.venv/share/jupyter/kernels/echolang3` and modify the string `"ipykernel_lancher"` in `argv` to say `"echo_kernel"`. Change the `language` to `"lang3"`.
8. Open the docs Jupyter Notebook, or any other Jupyter Notebook that you want, and select EchoLang3 from your venv. This may vary by which program you use to open the notebook.

Now when you run cells, it should output the exact contents of the cell!