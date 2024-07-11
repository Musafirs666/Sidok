﻿# TodoListBE

Generate new SqlServer Database
USE [sidok];
GO

-- Tabel dokter
CREATE TABLE dokter (
    id INT PRIMARY KEY IDENTITY,
    nip VARCHAR(50),
    nik VARCHAR(50),
    nama NVARCHAR(100),
    tanggal_lahir DATE,
    jenis_kelamin CHAR(1),
    tempat_lahir NVARCHAR(100)
);

-- Tabel poli
CREATE TABLE poli (
    id INT PRIMARY KEY,
    nama_poli NVARCHAR(100),
    lokasi NVARCHAR(100)
);

-- Tabel spesialisasi
CREATE TABLE spesialisasi (
    id INT PRIMARY KEY,
    nama NVARCHAR(100),
    gelar NVARCHAR(50)
);

-- Tabel bertugas_di (many-to-many antara dokter dan poli)
CREATE TABLE bertugas_di (
    hari NVARCHAR(50),
    dokter_id INT,
    poli_id INT,
    PRIMARY KEY (dokter_id, poli_id),
    FOREIGN KEY (dokter_id) REFERENCES dokter(id),
    FOREIGN KEY (poli_id) REFERENCES poli(id)
);

-- Tabel dokter_spesialisasi (many-to-many antara dokter dan spesialisasi)
CREATE TABLE dokter_spesialisasi (
    dokter_id INT,
    spesialisasi_id INT,
    PRIMARY KEY (dokter_id, spesialisasi_id),
    FOREIGN KEY (dokter_id) REFERENCES dokter(id),
    FOREIGN KEY (spesialisasi_id) REFERENCES spesialisasi(id)
);
