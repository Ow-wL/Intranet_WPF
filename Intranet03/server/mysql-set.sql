CREATE DATABASE intranet;
CREATE USER 'owl'@'localhost' IDENTIFIED BY 'CHANGE_ME';
GRANT ALL PRIVILEGES ON intranet.* TO 'owl'@'localhost';
FLUSH PRIVILEGES;

SELECT host, user FROM mysql.user WHERE user = 'root';

SHOW VARIABLES LIKE 'datadir';

-- 데이터 추가 -- 
insert into intranet.users(id, username, password, nickname) values ('3', '1', '1', '💦👻🐸얍')posts;

UPDATE intranet.posts
	SET category = '공지사항' 
	WHERE id = 3;
    
select * from intranet.posts;
select * from intranet.users;