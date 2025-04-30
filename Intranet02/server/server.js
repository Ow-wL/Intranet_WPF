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

// users 테이블이 없으면 생성 (기존 코드 유지)
db.run(`CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT,
    password TEXT
)`);

// 로그인 엔드포인트 추가
app.post('/login', (req, res) => {
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ status: 'error', message: '아이디와 패스워드 모두 입력해주세요.' });
    }

    db.get('SELECT id, username, password FROM users WHERE username = ?', [username], (err, row) => {
        if (err) {
            return res.status(500).json({ status: 'error', message: err.message });
        }

        if (!row) {
            return res.status(401).json({ status: 'error', message: 'ID를 확인해주세요.' });
        }

        // 비밀번호 비교 (현재는 평문 비교, 실제 서비스에서는 암호화하여 비교해야 함)
        if (row.password === password) {
            res.status(200).json({ status: 'success', message: 'Login successful', userId: row.id, username: row.username });
        } else {
            res.status(401).json({ status: 'error', message: '패스워드를 확인해주세요.' });
        }
    });
});

// 회원가입 엔드포인트
app.post('/register', (req, res) => {
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ status: 'error', message: 'Username and password are required' });
    }

    // 데이터베이스에서 동일한 username이 있는지 확인
    db.get('SELECT username FROM users WHERE username = ?', [username], (err, row) => {
        if (err) {
            return res.status(500).json({ status: 'error', message: err.message });
        }

        if (row) {
            // 이미 존재하는 username인 경우
            return res.status(409).json({ status: 'error', message: '이미 사용중인 ID 입니다.' });
        } else {
            // 존재하지 않는 username이므로 새로운 ID 생성 및 회원가입 진행
            db.get('SELECT MAX(id) AS lastId FROM users', (err, lastIdRow) => { // lastId 조회
                if (err) {
                    return res.status(500).json({ status: 'error', message: err.message });
                }
                const newId = (lastIdRow?.lastId || 0) + 1; // 새로운 ID 생성

                db.run('INSERT INTO users (id, username, password) VALUES (?, ?, ?)', [newId, username, password], function(err) {
                    if (err) {
                        return res.status(500).json({ status: 'error', message: err.message });
                    }
                    res.status(200).json({ status: 'success', id: this.lastID });
                });
            });
        }
    });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
