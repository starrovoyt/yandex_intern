# Test Yandex Task

0) Requirement - mono-msc:
```sh
$ sudo apt-get install mono-msc
```
or visit [MonoProject site](http://www.mono-project.com/download/) for Mac version
1) configure CreatingTables.cs by changing .db path (defaut - in the same dir with .exe) 
2) build solution by running .sh 
```sh
$ bash compile.sh
```
(alternative build)
```
$ mcs -out:Solution.exe source/CreatingTables.cs source/Methods.cs source/Task.cs -r:Mono.Data.Sqlite.dll -r:System.Data.dll
```
3) And run by:
```sh
$ mono Solution.exe tests/standart_test.tsv
```

### "Unit" test
in *tests/* you may see different test:
1) standart_test.tsv - 1000 correct random records
2) million_test.tsv - 2 * 10 ** 6 random correct records - everything is ok, works long, but steadily
3) wrong_test.tsv - 14 record, 9 uncorrect 
