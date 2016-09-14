# NSF2SQL

Convert Lotus notes (.nsf) to SQL

This application finds Lotus Notes applications on a Domino server or local computer and exports the documents to a SQL file or directly to a MySQL server.

The tables are forms with each document as a row.

It stores fields with multiple values as separate tables linked with a foreign key.

## Tutorial

### Download

1\. Click [Download as .zip] above.

### Open

2\. Unzip the file and go to \NSF2SQL\bin\Debug.

![](http://uzitech.github.io/images/nsf2sql7.png)

3\. Open NSF2SQL.exe.

### Search

4\. Click [Search Server] to search for an application on a Domino Server or [Search Computer] to browse your computer for a .nsf file.

![](http://uzitech.github.io/images/nsf2sql1.png)

5\. Enter the server information if searching a server or just the password if opening a local file.

![](http://uzitech.github.io/images/nsf2sql2.png)

### Select

6\. Select the database to export and click [Export Documents].

![](http://uzitech.github.io/images/nsf2sql3.png)

### Parse

7\. Wait until the program is done parsing the documents.

![](http://uzitech.github.io/images/nsf2sql4.png)

### Export

8\.Once all documents are parsed it will ask if you want to export to a server. Click [Yes] to have the sql imported directly to a MySQL server or click [No] to have the SQL dumped to a file so you can import it a different way.

![](http://uzitech.github.io/images/nsf2sql8.png)

9\. If you chose to have the program import it directly to a MySQL server it will ask for your server information. Database is the name you want for the new database

![](http://uzitech.github.io/images/nsf2sql6.png)

10\. If you chose to dump the SQL to a file it will be saved to a file called "export.sql" on your desktop and it will be opened after the program creates it.
