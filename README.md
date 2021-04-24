        converts fb2 files to text
		wrote by Anatoly Domnenko, Israel.

       place: href="https://github.com/adomnenk/FB2txt/
        
- fb2txt has no dependencies and does not require installation or any kind
- it convert a text during to try to find a text encoding related to a FB2 file.
- in the config file: fb2ToText.exe.config you have the folloing properties:

### Usage:
fb2ToText.exe <list of *.FB2 or folder wich contains these files> 

### configuration:
this app has config file: fb2ToText.exe.config, which apears in a root folder,
 1: key="is-save-in-folder" value="true"
	create or not, "text" folder in a current place to save output text files.
	
	2: key="maxLineLen" value="80"
	if the value > 0, then set maximum length of lines

### Examples:
In order to convert all fb2 files in `c:\books\
type in command line: $fb2ToText c:\books
=======
# FB2txt
it's converter FB2 to text format.

