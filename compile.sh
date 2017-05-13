#!/bin/bash
mcs -out:Solution.exe source/CreatingTables.cs source/Methods.cs source/Task.cs -r:Mono.Data.Sqlite.dll -r:System.Data.dll
