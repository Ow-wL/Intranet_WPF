// npm install express mysql2 body-parser cors
// D:
// cd CS_WS\Intranet03\Intranet03\server 
// node server.js

const mysql = require('mysql2');
const express = require('express');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());

// 데이터베이스 연결
const db = mysql.createConnection({
    host: 'localhost',
    user: 'owl',
    password: '0809',
    database: 'intranet'
});

db.connect((err) => {
    if (err) {
        console.error('error connecting: ' + err.stack);
        return;
    }
    console.log('Connected to the intranet database for login.');
});

// users 테이블이 없으면 생성
db.query(`CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) UNIQUE,
    password VARCHAR(255),
    nickname VARCHAR(255) UNIQUE
)`, (err, results) => {
    if (err) {
        console.error(err.message);
    }
});

// 로그인 엔드포인트
app.post('/login', (req, res) => {
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ status: 'error', message: '아이디와 패스워드 모두 입력해주세요.' });
    }

    db.query('SELECT id, username, password, nickname FROM users WHERE username = ?', [username], (err, results) => {
        if (err) {
            return res.status(500).json({ status: 'error', message: err.message });
        }

        if (results.length === 0) {
            return res.status(401).json({ status: 'error', message: 'ID를 확인해주세요.' });
        }

        const user = results[0];
        if (user.password === password) {
            res.status(200).json({ status: 'success', message: 'Login successful', userId: user.id, username: user.username, nickname: user.nickname });
        } else {
            return res.status(401).json({ status: 'error', message: '패스워드를 확인해주세요.' });
        }
    });
});

// 회원가입 엔드포인트
app.post('/register', (req, res) => {
    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        return res.status(400).json({ status: 'error', message: 'Username, Password, and Nickname are required' });
    }

    // username 중복 확인
    db.query('SELECT username FROM users WHERE username = ?', [username], (err, results) => {
        if (err) {
            return res.status(500).json({ status: 'error', message: err.message });
        }

        if (results.length > 0) {
            return res.status(409).json({ status: 'error', message: '이미 사용중인 ID 입니다.' });
        } else {
            // nickname 중복 확인
            db.query('SELECT nickname FROM users WHERE nickname = ?', [nickname], (err, results) => {
                if (err) {
                    return res.status(500).json({ status: 'error', message: err.message });
                }

                if (results.length > 0) {
                    return res.status(409).json({ status: 'error', message: '이미 사용중인 별명 입니다.' });
                } else {
                    // username과 nickname 모두 중복되지 않으면 회원가입 진행
                    db.query('INSERT INTO users (username, password, nickname) VALUES (?, ?, ?)', [username, password, nickname], (err, results) => {
                        if (err) {
                            return res.status(500).json({ status: 'error', message: err.message });
                        }
                        res.status(200).json({ status: 'success', id: results.insertId, nickname: nickname });
                    });
                }
            });
        }
    });
});

// 서버 실행
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});

