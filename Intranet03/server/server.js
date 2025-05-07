// D:
// cd D:\CS_WS\Intranet03\Intranet03\server
// node server.js
// npm install express mysql2 moment-timezone body-parser cors

const express = require('express');
const mysql = require('mysql2/promise');
const moment = require('moment-timezone');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());

// 데이터베이스 연결 설정
const dbConfig = {
    host: 'localhost',
    user: 'owl',
    password: '0809',
    database: 'intranet'
};

let dbPool; 

// 데이터베이스 연결 함수 
async function createDbPool() {
    try {
        dbPool = mysql.createPool(dbConfig); 
        console.log('Database connected');
    } catch (error) {
        console.error('Database connection failed:', error);
        throw error;
    }
}

// 서버 시작 시 데이터베이스 연결 및 테이블 생성 (async 즉시 실행 함수)
(async () => {
    try {
        await createDbPool(); 

        // users 테이블이 없으면 생성
        await dbPool.execute(`CREATE TABLE IF NOT EXISTS users (
            id INT AUTO_INCREMENT PRIMARY KEY,
            username VARCHAR(255) UNIQUE,
            password VARCHAR(255),
            nickname VARCHAR(255) UNIQUE
        )`);

        // posts 테이블이 없으면 생성 
        await dbPool.execute(`CREATE TABLE IF NOT EXISTS posts (
            id INT AUTO_INCREMENT PRIMARY KEY,
            author VARCHAR(255) NOT NULL, 
            title VARCHAR(255) NOT NULL,
            content TEXT NOT NULL,
            category VARCHAR(50),
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            view_count INT DEFAULT 0
        )`);

        // 서버 실행
        const PORT = process.env.PORT || 3000;
        app.listen(PORT, () => {
            console.log(`Server is running on port ${PORT}`);
        });

    } catch (error) {
        console.error('Server startup failed:', error);
    }
})();


// 로그인 엔드포인트
app.post('/login', async (req, res) => {
    const { username, password } = req.body;
    if (!username || !password) {
        return res.status(400).json({ status: 'error', message: '아이디와 패스워드 모두 입력해주세요.' });
    }

    try {
        const [results] = await dbPool.execute('SELECT id, username, password, nickname FROM users WHERE username = ?', [username]);

        if (results.length === 0) {
            return res.status(401).json({ status: 'error', message: 'ID를 확인해주세요.' });
        }

        const user = results[0];
        if (user.password === password) {
            res.status(200).json({ status: 'success', message: 'Login successful', userId: user.id, username: user.username, nickname: user.nickname });
        } else {
            return res.status(401).json({ status: 'error', message: '패스워드를 확인해주세요.' });
        }
    } catch (err) {
        return res.status(500).json({ status: 'error', message: err.message });
    }
});

// 회원가입 엔드포인트
app.post('/register', async (req, res) => {
    const { username, password, nickname } = req.body;
    if (!username || !password || !nickname) {
        return res.status(400).json({ status: 'error', message: 'Username, Password, and Nickname are required' });
    }

    try {
        // username 중복 확인
        const [usernameResults] = await dbPool.execute('SELECT username FROM users WHERE username = ?', [username]);
        if (usernameResults.length > 0) {
            return res.status(409).json({ status: 'error', message: '이미 사용중인 ID 입니다.' });
        }

        // nickname 중복 확인
        const [nicknameResults] = await dbPool.execute('SELECT nickname FROM users WHERE nickname = ?', [nickname]);
        if (nicknameResults.length > 0) {
            return res.status(409).json({ status: 'error', message: '이미 사용중인 별명 입니다.' });
        }

        // username과 nickname 모두 중복되지 않으면 회원가입 진행
        const [insertResults] = await dbPool.execute('INSERT INTO users (username, password, nickname) VALUES (?, ?, ?)', [username, password, nickname]);

        res.status(200).json({ status: 'success', id: insertResults.insertId, nickname: nickname });

    } catch (err) {
        return res.status(500).json({ status: 'error', message: err.message });
    }
});

// 게시글 저장 엔드포인트
app.post('/posts', async (req, res) => {
    const { author, title, content, category } = req.body;

    if (!category || !title || !content) {
        return res.status(400).json({ status: 'error', message: '제목, 내용, 카테고리를 모두 입력해주세요.' });
    }

    try {
        const [results] = await dbPool.execute('INSERT INTO posts (author, title, content, category, created_at) VALUES (?, ?, ?, ?, CONVERT_TZ(UTC_TIMESTAMP(), "+00:00", "+09:00"))', [author, title, content, category]);

        res.status(201).json({ status: 'success', message: '게시글이 성공적으로 저장되었습니다.', postId: results.insertId });
    } catch (err) {
        return res.status(500).json({ status: 'error', message: err.message });
    }
});

// 게시글 조회 엔드포인트
// 게시글 조회 엔드포인트 수정 (검색 기능 추가)
app.get('/posts', async (req, res) => {
    try {
        // 검색 파라미터 가져오기
        const { startDate, endDate, category, title, author } = req.query;
        
        // 기본 쿼리
        let query = 'SELECT id, author, title, content, category, created_at, view_count AS Views FROM posts';
        let conditions = [];
        let params = [];
        
        // 날짜 검색 조건
        if (startDate) {
            if (endDate) {
                // 날짜 범위 검색
                conditions.push('DATE(created_at) BETWEEN ? AND ?');
                params.push(startDate, endDate);
            } else {
                // 단일 날짜 검색
                conditions.push('DATE(created_at) = ?');
                params.push(startDate);
            }
        }
        
        // 카테고리 검색 조건
        if (category && category !== '전체') {
            conditions.push('category = ?');
            params.push(category);
        }
        
        // 제목 검색 조건
        if (title) {
            conditions.push('title LIKE ?');
            params.push(`%${title}%`);
        }
        
        // 작성자 검색 조건
        if (author) {
            conditions.push('author LIKE ?');
            params.push(`%${author}%`);
        }
        
        // 조건이 있으면 WHERE 절 추가
        if (conditions.length > 0) {
            query += ' WHERE ' + conditions.join(' AND ');
        }
        
        // 정렬 추가
        query += ' ORDER BY created_at DESC';
        
        const [rows] = await dbPool.execute(query, params);
        
        const postsWithKoreanTime = rows.map(post => {
            const koreanCreatedAt = moment.utc(post.created_at).tz('Asia/Seoul').format('YYYY-MM-DDTHH:mm:ss');
            return {
                id: post.id,
                author: post.author,
                title: post.title,
                content: post.content,
                category: post.category,
                Date: koreanCreatedAt,
                Views: post.Views
            };
        });
        
        res.status(200).json(postsWithKoreanTime);
    } catch (err) {
        return res.status(500).json({ status: 'error', message: err.message });
    }
});

// 게시글 삭제 엔드포인트
app.delete('/posts/:id', async (req, res) => {
    const postId = req.params.id;
    const author = req.query.author;

    if (!author) {
        return res.status(400).json({ status: 'error', message: '작성자 정보가 필요합니다.' });
    }

    try {
        // 게시글을 찾고 작성자 확인
        const [findPostResults] = await dbPool.execute('SELECT author FROM posts WHERE id = ?', [postId]);
        if (findPostResults.length === 0) {
            return res.status(404).json({ status: 'error', message: '게시글을 찾을 수 없습니다.' });
        }

        const post = findPostResults[0];
        if (post.author !== author) {
            return res.status(403).json({ status: 'error', message: '삭제 권한이 없습니다.' });
        }

        // 게시글 삭제
        await dbPool.execute('DELETE FROM posts WHERE id = ?', [postId]);

        res.status(200).json({ status: 'success', message: '게시글이 삭제되었습니다.' });

    } catch (err) {
        return res.status(500).json({ status: 'error', message: err.message });
    }
});