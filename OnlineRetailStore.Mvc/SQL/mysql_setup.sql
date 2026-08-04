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

INSERT IGNORE INTO categories (name, description) VALUES
  ('Apparel', 'Everyday layers, made to last.'),
  ('Gadgets', 'Small tech that makes a big difference.'),
  ('Tools', 'Built for the job, priced fair.');

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

INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Heavyweight Flannel Overshirt', id, 'Warm layered flannel overshirt', 'A heavyweight flannel overshirt built for cold-weather layering.', 2450, 14, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Apparel';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Everyday Fleece Joggers', id, 'Soft fleece joggers for daily wear', 'Comfortable everyday fleece joggers with an elastic waistband.', 1650, 22, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Apparel';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Waxed Canvas Tote', id, 'Durable waxed canvas carryall', 'A rugged waxed canvas tote built to carry everything you need.', 1200, 9, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Apparel';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Merino Wool Crew Sweater', id, 'Soft merino wool crew sweater', 'A breathable merino wool sweater that keeps you warm without the bulk.', 3200, 6, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Apparel';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'AirFlow Wireless Earbuds', id, 'Comfortable true wireless earbuds', 'True wireless earbuds with long battery life and a comfortable fit.', 4200, 18, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Gadgets';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'PulseFit Smart Band', id, 'Fitness tracker with heart-rate monitor', 'A lightweight fitness band with heart-rate monitoring and notifications.', 3600, 11, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Gadgets';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT '65W GaN Fast Charger', id, 'Compact high-speed GaN charger', 'A compact 65W GaN charger that fast-charges phones and laptops alike.', 2100, 30, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Gadgets';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'BoomBox Mini Speaker', id, 'Portable Bluetooth speaker', 'A pocket-sized Bluetooth speaker with surprisingly big sound.', 3900, 8, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Gadgets';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT '20000mAh Power Bank', id, 'High-capacity portable charger', 'A 20000mAh power bank with fast charging for phones and tablets.', 2800, 0, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Gadgets';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Cordless Drill Starter Kit', id, 'Cordless drill with starter bit set', 'A cordless drill starter kit ready for most household projects.', 6500, 7, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Tools';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT '48-Slot Tool Organizer', id, 'Compact modular tool organizer', 'A 48-slot organizer that keeps small tools and parts sorted.', 1450, 16, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Tools';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Adjustable Multi-Wrench Set', id, 'Adjustable wrench set for tight spots', 'A set of adjustable wrenches that cover most household repairs.', 1980, 12, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Tools';
INSERT IGNORE INTO products (name, category_id, short_description, description, price, stock, image_url)
SELECT 'Reinforced Work Gloves', id, 'Durable reinforced work gloves', 'Reinforced work gloves that hold up to daily job-site wear.', 950, 25, '~/Content/Images/placeholder.svg' FROM categories WHERE name = 'Tools';

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

-- Seed example accounts (uses MySQL SHA2 to store SHA-256 hash of the cleartext password)
-- Passwords used here are for development only. Change them in production.
INSERT IGNORE INTO users (username, passwordhash, email, role, is_approved) VALUES
  ('admin', SHA2('AdminPass123!', 256), 'admin@example.com', 'Admin', 1),
  ('vendor', SHA2('VendorPass123!', 256), 'vendor@example.com', 'Vendor', 1),
  ('alice', SHA2('UserPass123!', 256), 'alice@example.com', 'User', 1);
