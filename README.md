# openstreetmap2oracle
export openstreetmap *.osm files to oracle database


<b>Prerequisites</b>

Oracle database client
If you want to use OpenStreetMap2Oracle, you first have to download and install the oracle database client. You don't need to install the client, if you are running OpenStreetMap2Oracle on the oracle database server machine itself. Then you can go to the task Installation of OpenStreetMap2Oracle. You have to take care, if you are using OpenStreetMap2Oracle in 32Bit, you have to download the oracle client in 32Bit. Otherwise you have to download the 64Bit oracle client. Currently only 32Bit versions of OpenStreetMap2Oracle are available. 64Bit versions will follow. If you have downloaded the oracle client, you have to install it. We recommend the version "Administrator". Minimum version is "Instant Client". After installation, you need to configure the tnsnames.ora file for your oracle database server. OpenStreetMap2Oracle will ask you for this servicename. Make sure you are able to tnsping your oracle database server. 
Oracle database
You will need a database installation of oracle. The version doesn't matter. You do not need oracle spatial! Oracle locator is enough to save geometries. Usually, in a standard database installation, there is oracle locator installed. We don't take a closer look on installing and configuring oracle database here.

<b>Installation of OpenStreetMap2Oracle</b>

If you have downloaded the Installer archive from downloads, you have to unzip the file. After this, you have to execute the setup.exe. The installer will start to work. You will need Microsoft .NET Framework 4.0. If it is not installed on your machine, the installer asks you to download and install it for you. This may take several minutes. After finishing installation, the software appears in your start menu. You can uninstall it via standard uninstall - software from windows. Now you can use OpenStreetMap2Oracle.

Tip: If you already have installed OpenStreetMap2Oracle and you downloaded a newer release, the installer updates your old version of OpenStreetMap2Oracle.

<b>Setting up the database</b>

If you have installed OpenStreetMap2Oracle and oracle client you can nearly start to export your first data. The last step is to set up a database schema for exports. 
Therefore we provide a sample SQL script for creating a database user. You have to run this script with system privileges, e.g. as user SYSTEM:<br />
<code>
Create user USERNAME identified by PASSWORD default tablespace TABLESPACE_NAME;
Alter user USERNAME quota unlimited on TABLESPACE_NAME;<br /> 
Alter user USERNAME quota unlimited on INDEX_TABLESPACE_NAME;<br /> 
GRANT CREATE TABLE TO USERNAME ;<br /> 
GRANT CREATE VIEW TO USERNAME ;<br /> 
GRANT CREATE SESSION TO USERNAME ;<br /> 
grant execute on MDSYS.SPATIAL_INDEX to USERNAME ;<br /> 
COMMIT;<br />
</code>
If the user was created, you can go on with creating the 4 base tables (point, line, polygon, roads) of the data model. If you have downloaded the Installer archive, there is an archive with 4 sql scripts inside (SQL.zip). Each sql script creates a table, e.g. the planet_osm_point table. 

If you have created the 4 base tables, you can export your OpenStreetMap data into the database, have a look at page Creating your first export.

<b>Creating your first export</b>

If you have successfully installed and configured all components (oracle client, Microsoft .NET Framework 4) and the application itself, you can export your first *.osm - file to oracle.
You can download extracts from OpenStreetMap directly via the api from OpenStreetMap or you can download datasets from geofabrik.
Start the application and click on Verbindung - Oracle verbinden
Oracle Verbindung
The login screen appears, enter your oracle username (schema), the password and the oracle service name (tnsnames.ora)
Login.png
The Message "Open" appears, if the user credentials for connections are ok. After this, you can click on Datei - öffnen to open a *.osm - file. After opening the file, the application should start to export the points, lines and polygons of the file into the oracle database. Optionally you can enable/disable the checkbox Oracle SQL Fehlermeldungen anzeigen, this controls, if the sql errors from the database are displayed in the textbox of the application.

<b>Performance</b>

Currently, no threading of export is supported. We are on the way to speed up the export queue. Multithreading comes within the next versions. The speed of export depends on the hardware of the database server and the connection between the export client and the server. There is the option to run the software directly on the oracle database server. Furthermore, the time needed to export a file depends on the size of the OSM - file as well. If it is a large file, the time to compute it is longer than a small file. We don't have any performance tests yet, so we can not give any information how long computing data sets may take. We suggest to start with smaller extracts. The export time does not depend on the RAM of the exporting client, because nothing is stored or computed in memory. The application works with a minimum of available memory.
<b>Performance update</b>
The actual version does provide the option to compute all data in memory. So the performance speeds up with the amount of avaiable RAM. See parameter <i>slim_mode</i>.

<b>Size of tablespaces</b>

If you are about to export a file, which is bigger than 32GB, you may encounter errors in oracle database. This is because of the limitation of standard oracle tablespace size to 32GB. If you want to export bigger files, you have to create a BIGFILE - tablespace in oracle. Once you have created such a tablespace and assigned it to a user, you dont have any limitations in size of tablespace.

<b>Corrupt geometries</b>

Geometry definitions can differ from database to database. So, oracle has different methods or specifications to store geometries. They are different from OpenStreetMap. OpenStreetMap2Oracle corrects this differences while exporting the data. Anyway it happens, that specific geometries, especially polygons are exported to oracle in a wrong way. We provide some SQL scripts, to delete or correct this objects. After this, the spatial index should be built correctly. 
Furthermore we permanently enhance the recognition of this errors while exporting the data.

<b>Spatial Indexing</b>

Spatial indexing is essential, if you want to work with your data. So, each of the 4 base tables have to get a spatial index. If all geometries in this tables are correct, the spatial index is built correctly. We provide a SQL script to build a spatial index on each table. 
