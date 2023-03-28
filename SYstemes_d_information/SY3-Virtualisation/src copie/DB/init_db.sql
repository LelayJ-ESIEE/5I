use QuizDB;

CREATE TABLE IF NOT EXISTS "answer" (
	"id"	INTEGER,
	"text"	TEXT NOT NULL,
	"is_correct"	INTEGER NOT NULL,
	"question_id"	INTEGER NOT NULL,
	FOREIGN KEY("question_id") REFERENCES "question"("id"),
	PRIMARY KEY("id")
)

CREATE TABLE IF NOT EXISTS "participation" (
	"id"	INTEGER,
	"playerName"	TEXT,
	"score"	INTEGER,
	"date"	INTEGER
)

CREATE TABLE IF NOT EXISTS "question" (
	"id"	INTEGER,
	"title"	TEXT NOT NULL,
	"text"	TEXT NOT NULL,
	"position"	INTEGER NOT NULL,
	"image"	TEXT,
	PRIMARY KEY("id")
)