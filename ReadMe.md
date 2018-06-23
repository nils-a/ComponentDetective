# ComponentDetective
This tool parses folders for components used in code.

My "Solutions" always follow a certain folder-structure and separation of components:

 - bin 
	 - Every output ends up here
 - src
	 - Solution1
	 - Solution2
	 - ...
 - lib
	 - ExternalLib1
	 - ExternalLib2
	 - nuget-packages


With this comes nicely separated components and small solutions. (i.e. Mostly there are only two projects in each solution: the component in question and it's tests..)

But this layout lacks some overall-traceablility like

 - Which component references witch other component (ideally every component references only "contracts" with "start" being the exception and referencing every other component)
 - Which external dll's are referenced somewhere
 - Which nuget-packages are referenced in which version

ComponentDetective is the tool to answer the above questions... 