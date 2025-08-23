CREATE DATABASE IF NOT EXISTS xpe_arq_desafio_final
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE xpe_arq_desafio_final;

CREATE TABLE IF NOT EXISTS products (
  id          INT AUTO_INCREMENT PRIMARY KEY,
  name        VARCHAR(100) NOT NULL,
  description VARCHAR(500) NOT NULL,
  price       DECIMAL(18,2) NOT NULL,
  active      TINYINT(1) NOT NULL DEFAULT 0,
  created_at  DATETIME NOT NULL,
  updated_at  DATETIME NULL,
  INDEX idx_products_active (active),
  INDEX idx_products_created_at (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;