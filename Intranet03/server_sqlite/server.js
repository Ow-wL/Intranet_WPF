// D:
// cd D:\CS_WS\Intranet03\Intranet03\server
// node server.js
const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());

// 데이터베이스 연결
const db = new sqlite3.Database('USER.DB', (err) => {
    if (err) {
        console.error(err.message);
    }
    console.log('Connected to the USER.DB database for login.');
});

// users 테이블이 없으면 생성 
db.run(`CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT UNIQUE,
    password TEXT,
    nickname TEXT UNIQUE
)`);

// 로그인 엔드포인트
app.post('/login', (req, res) => {
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ status: 'error', message: '아이디와 패스워드 모두 입력해주세요.' });
    }

    db.get('SELECT id, username, password, nickname FROM users WHERE username = ?', [username], (err, row) => {
        if (err) {
            return res.status(500).json({ status: 'error', message: err.message });
        }

        if (!row) {
            return res.status(401).json({ status: 'error', message: 'ID를 확인해주세요.' });
        }

        if (row.password === password) {
            // 로그인 성공 시 userId, username, nickname을 함께 응답
            res.status(200).json({ status: 'success', message: 'Login successful', userId: row.id, username: row.username, nickname: row.nickname });
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
    db.get('SELECT username FROM users WHERE username = ?', [username], (err, usernameRow) => {
        if (err) {
            return res.status(500).json({ status: 'error', message: err.message });
        }

        if (usernameRow) {
            return res.status(409).json({ status: 'error', message: '이미 사용중인 ID 입니다.' });
        } else {
            // nickname 중복 확인
            db.get('SELECT nickname FROM users WHERE nickname = ?', [nickname], (err, nicknameRow) => {
                if (err) {
                    return res.status(500).json({ status: 'error', message: err.message });
                }

                if (nicknameRow) {
                    return res.status(409).json({ status: 'error', message: '이미 사용중인 별명 입니다.' });
                } else {
                    // username과 nickname 모두 중복되지 않으면 회원가입 진행
                    db.get('SELECT MAX(id) AS lastId FROM users', (err, lastIdRow) => {
                        if (err) {
                            return res.status(500).json({ status: 'error', message: err.message });
                        }
                        const newId = (lastIdRow?.lastId || 0) + 1;

                        db.run('INSERT INTO users (id, username, password, nickname) VALUES (?, ?, ?, ?)', [newId, username, password, nickname], function(err) {
                            if (err) {
                                return res.status(500).json({ status: 'error', message: err.message });
                            }
                            res.status(200).json({ status: 'success', id: this.lastID, nickname: nickname });
                        });
                    });
                }
            });
        }
    });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
