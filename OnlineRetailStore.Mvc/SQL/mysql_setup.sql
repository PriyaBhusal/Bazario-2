-- MySQL setup script for OnlineRetailStore (XAMPP)
-- Run in phpMyAdmin or mysql client

CREATE DATABASE IF NOT EXISTS onlineretail CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE onlineretail;

CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(100) NOT NULL UNIQUE,
  passwordhash VARCHAR(128) NOT NULL,
  email VARCHAR(255) NOT NULL,
	role VARCHAR(50) DEFAULT 'User',
  is_approved TINYINT(1) DEFAULT 1,
  phone VARCHAR(30) NULL,
  address VARCHAR(500) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Manage Profile: add contact/delivery fields to an existing users table
ALTER TABLE users ADD COLUMN IF NOT EXISTS phone VARCHAR(30) NULL;
ALTER TABLE users ADD COLUMN IF NOT EXISTS address VARCHAR(500) NULL;

CREATE TABLE IF NOT EXISTS categories (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(100) NOT NULL UNIQUE,
  description VARCHAR(500) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS products (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(200) NOT NULL UNIQUE,
  category_id INT NOT NULL,
  vendor_id INT NULL,
  short_description VARCHAR(300) NULL,
  description TEXT NULL,
  price DECIMAL(10,2) NOT NULL DEFAULT 0,
  stock INT NOT NULL DEFAULT 0,
  image_url VARCHAR(500) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_products_category FOREIGN KEY (category_id) REFERENCES categories(id),
  CONSTRAINT fk_products_vendor FOREIGN KEY (vendor_id) REFERENCES users(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Manage Products: vendor ownership on an existing products table (NULL = store-owned)
ALTER TABLE products ADD COLUMN IF NOT EXISTS vendor_id INT NULL;

CREATE TABLE IF NOT EXISTS orders (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  status VARCHAR(30) NOT NULL DEFAULT 'Pending',
  total DECIMAL(10,2) NOT NULL DEFAULT 0,
  payment_method VARCHAR(30) NOT NULL DEFAULT 'eSewa',
  transaction_id VARCHAR(100) NULL,
  shipping_address VARCHAR(500) NULL,
  shipping_phone VARCHAR(30) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_orders_user FOREIGN KEY (user_id) REFERENCES users(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Payment Support: mock eSewa transaction reference on an existing orders table
ALTER TABLE orders ADD COLUMN IF NOT EXISTS payment_method VARCHAR(30) NOT NULL DEFAULT 'eSewa';
ALTER TABLE orders ADD COLUMN IF NOT EXISTS transaction_id VARCHAR(100) NULL;
ALTER TABLE orders ADD COLUMN IF NOT EXISTS payment_ref VARCHAR(100) NULL;

CREATE TABLE IF NOT EXISTS order_items (
  id INT AUTO_INCREMENT PRIMARY KEY,
  order_id INT NOT NULL,
  product_id INT NOT NULL,
  product_name VARCHAR(200) NOT NULL,
  quantity INT NOT NULL,
  unit_price DECIMAL(10,2) NOT NULL,
  CONSTRAINT fk_order_items_order FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
  CONSTRAINT fk_order_items_product FOREIGN KEY (product_id) REFERENCES products(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS notifications (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  message VARCHAR(500) NOT NULL,
  is_read TINYINT(1) NOT NULL DEFAULT 0,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_notifications_user FOREIGN KEY (user_id) REFERENCES users(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ratings (
  id INT AUTO_INCREMENT PRIMARY KEY,
  product_id INT NOT NULL,
  order_id INT NOT NULL,
  user_id INT NOT NULL,
  rating TINYINT NOT NULL,
  comment VARCHAR(1000) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_ratings_product FOREIGN KEY (product_id) REFERENCES products(id),
  CONSTRAINT fk_ratings_order FOREIGN KEY (order_id) REFERENCES orders(id),
  CONSTRAINT fk_ratings_user FOREIGN KEY (user_id) REFERENCES users(id),
  CONSTRAINT chk_ratings_range CHECK (rating BETWEEN 1 AND 5),
  CONSTRAINT uq_ratings_order_product UNIQUE (order_id, product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS wishlists (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  product_id INT NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_wishlists_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  CONSTRAINT fk_wishlists_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
  CONSTRAINT uq_wishlists_user_product UNIQUE (user_id, product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Previously enforced unique emails (removed so multiple accounts may share an email)
-- ALTER TABLE users ADD UNIQUE KEY uq_users_email (email);
-- Re-enable unique email constraint so emails must be unique for login-by-email
ALTER TABLE users ADD UNIQUE KEY uq_users_email (email);

-- Example: create admin manually (replace hashed_password_here with SHA256 of your password)
-- INSERT INTO users (username, passwordhash, email, role) VALUES ('admin', 'hashed_password_here', 'admin@example.com', 'Admin');

-- Seed admin account only (uses MySQL SHA2 to store SHA-256 hash of the cleartext password)
-- Password used here is for development only. Change it in production.
INSERT IGNORE INTO users (username, passwordhash, email, role, is_approved) VALUES
  ('admin', SHA2('AdminPass123!', 256), 'admin@example.com', 'Admin', 1);
