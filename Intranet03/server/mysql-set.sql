CREATE DATABASE intranet;
CREATE USER 'owl'@'localhost' IDENTIFIED BY '0809';
GRANT ALL PRIVILEGES ON intranet.* TO 'owl'@'localhost';
FLUSH PRIVILEGES;


SELECT host, user FROM mysql.user WHERE user = 'root';