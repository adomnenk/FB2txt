<p align="center">
    <h1 align="center">fb2txt converter</h1>
    <p align="center">
        converts fb2 files to text
		wrote by Anatoly Domnenko, Israel
	</p>
    <p align="center">
        <a href="https://github.com/adomnenk/FB2txt/"/></a>
        
    </p>
    <hr>
</p>

- fb2txt has no dependencies and does not require installation or any kind

### Usage:
fb2ToText.exe <list of *.FB2 or folder wich contains these files> 

### configuration:
this util configure via fb2ToText.exe.config file by the following elements:
 key="is-save-in-folder" value="true"
	set if keep output files into folder text under current filder or keep it in same folder as input file
	
	key="maxLineLen" value="80"
	set maximum length of line

### Examples:
In order to convert all fb2 files in `c:\books\
type in command line: $fb2ToText c:\books
